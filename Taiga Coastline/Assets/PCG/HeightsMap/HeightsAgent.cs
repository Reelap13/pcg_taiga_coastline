using PCG_Map.Algorithms;
using PCG_Map.Algorithms.Linear;
using PCG_Map.Algorithms.PerlinNoise;
using PCG_Map.Algorithms.Voronoi;
using PCG_Map.New_Bioms;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static PCG_Map.Algorithms.HeightsmapAlgorithmsData;

namespace PCG_Map.Heights
{
    public class HeightsAgent : Singleton<HeightsAgent>
    {
        [SerializeField] private Coastline _coastline;
        [SerializeField] private LandHeights _land_heights;
        [SerializeField] private SeaHeights _sea_heights;

        [field: SerializeField]
        public float TerrainHeight { get; private set; } = 600;
        [field: SerializeField]
        public float SeaHeight { get; private set; } = 50;
        [field: SerializeField]
        public float MinHeight { get; private set; } = 1;

        public BiomsController BiomsController => Generator.Instance.Bioms;

        private HeightsmapAlgorithmsData _algorithms;

        public void Initialize()
        {
            // Change 10 const to number of bioms templates in the future!!!
            NativeHashMap<int, HeightsMapAlgorithmData> data = new NativeHashMap<int, HeightsMapAlgorithmData>(10, Allocator.Persistent);
            NativeList<PerlinNoiseAlgorithm> perlin_noise_algorithms = new NativeList<PerlinNoiseAlgorithm>(Allocator.Persistent);
            NativeList<LinearFunction> linear_functions = new NativeList<LinearFunction>(Allocator.Persistent);

            _algorithms = new HeightsmapAlgorithmsData() { 
                AlgorithmsData = data,
                PerlinNoiseAlgorithms = perlin_noise_algorithms,
                LinearFunctions = linear_functions
            };
        }

        public void RegisterBiomTemplate(int biom_template_id, HeightMapAlgorithmType type, IHeightsMapAlgorithm algorithm)
        {
            _algorithms.AddAlgorithm(biom_template_id, type, algorithm);
        }

        private void OnDestroy()
        {
            _algorithms.Dispose();
        }


        public FindHeights GetHeightMap(float2 start_position, int size, float step, NativeArray<int> bioms_id, NativeArray<float> heights)
        {
            var job = new FindHeights
            {
                StartPosition = start_position,
                Size = size,
                Step = step,
                Algorithms = _algorithms,
                BiomsId = bioms_id,
                Heights = heights
            };

            return job;
            /*JobHandle jobHandle = job.Schedule(heights_map_resolution * heights_map_resolution, 64);
            jobHandle.Complete();

            bioms_id.Dispose();
            return heights;*/
        }

        /*public Matrix2D GetHeights(Vector2 start_position, Vector2Int size, int heights_map_resolution)
        {
            Matrix2D heights = new(heights_map_resolution);
            Vector2 position = start_position;
            for (int i = 0; i < heights_map_resolution; ++i)
            {
                position.x = start_position.x + size.x * i / (heights_map_resolution - 1);
                for (int j = 0; j < heights_map_resolution; ++j)
                {
                    position.y = start_position.y + size.y * j / (heights_map_resolution - 1);
                    heights[i, j] = GetHeight(position) / TerrainHeight;
                }
            }


            return heights;
        }*/

        public float GetHeight(Vector2 position)
        {
            if (GetOffsetsFromCoastline(position) >= 0)
                return _land_heights.GetHeight(position);
            else return _sea_heights.GetHeight(position);
        }

        public float GetOffsetsFromCoastline(Vector2 position)
        {
            float coastline_y = _coastline.GetYValue(position.x);
            return position.y - coastline_y;
        }

        /*public void Initialize()
        {
            _coastline.Initialize();
        }*/
    }


    [BurstCompile]
    public struct FindHeights : IJobParallelFor
    {
        [ReadOnly] public float2 StartPosition;
        [ReadOnly] public int Size;
        [ReadOnly] public float Step;
        [ReadOnly] public HeightsmapAlgorithmsData Algorithms;
        [ReadOnly] public NativeArray<int> BiomsId;
        [WriteOnly] public NativeArray<float> Heights;

        public void Execute(int index)
        {
            int i = index / Size;
            int j = index % Size;
            
            float2 position = StartPosition + new float2(i, j) * Step;
            Heights[index] = Algorithms.CalculateHeight(position, BiomsId[index]);
        }
    }
}