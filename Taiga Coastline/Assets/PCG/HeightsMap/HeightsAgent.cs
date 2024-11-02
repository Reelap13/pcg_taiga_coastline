using PCG_Map.Algorithms;
using PCG_Map.Algorithms.Linear;
using PCG_Map.Algorithms.PerlinNoise;
using PCG_Map.New_Bioms;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Heights
{
    public class HeightsAgent : Singleton<HeightsAgent>
    {
        [field: SerializeField]
        public float TerrainHeight { get; private set; } = 600;
        [field: SerializeField]
        public float SeaHeight { get; private set; } = 50;
        [field: SerializeField]
        public float MinHeight { get; private set; } = 1;

        public BiomsController BiomsController => Generator.Instance.Bioms;

        private HeightMapAlgorithmsData _algorithms;

        public void Initialize()
        {
            NativeHashMap<int, HeightMapAlgorithmData> data = new NativeHashMap<int, HeightMapAlgorithmData>(BiomsController.BiomsTemplatesNumber, Allocator.Persistent);
            NativeList<PerlinNoiseAlgorithm> perlin_noise_algorithms = new NativeList<PerlinNoiseAlgorithm>(Allocator.Persistent);
            NativeList<MultiPerlinNoiseAlgorithm> multi_perlin_noise_algorithms = new NativeList<MultiPerlinNoiseAlgorithm>(Allocator.Persistent);
            NativeList<LinearFunction> linear_functions = new NativeList<LinearFunction>(Allocator.Persistent);

            _algorithms = new HeightMapAlgorithmsData() { 
                AlgorithmsData = data,
                PerlinNoiseAlgorithms = perlin_noise_algorithms,
                MultiPerlinNoiseAlgorithms = multi_perlin_noise_algorithms,
                LinearFunctions = linear_functions
            };
        }

        public void RegisterBiomTemplate(int biom_template_id, HeightMapAlgorithmType type, IHeightMapAlgorithm algorithm)
        {
            _algorithms.AddAlgorithm(biom_template_id, type, algorithm);
        }

        private void OnDestroy()
        {
            _algorithms.Dispose();
        }


        public FindHeights GetHeightMap(float2 start_position, int size, float step, 
            NativeArray<PointBiom> biom_map, NativeArray<float> heights)
        {
            var job = new FindHeights
            {
                StartPosition = start_position,
                Size = size,
                Step = step,
                Algorithms = _algorithms,
                BiomMap = biom_map,
                Heights = heights
            };

            return job;
        }
    }


    [BurstCompile]
    public struct FindHeights : IJobParallelFor
    {
        [ReadOnly] public float2 StartPosition;
        [ReadOnly] public int Size;
        [ReadOnly] public float Step;
        [ReadOnly] public HeightMapAlgorithmsData Algorithms;
        [ReadOnly] public NativeArray<PointBiom> BiomMap;
        [WriteOnly] public NativeArray<float> Heights;

        public void Execute(int index)
        {
            int i = index / Size;
            int j = index % Size;
            
            float2 position = StartPosition + new float2(i, j) * Step;
            Heights[index] = Algorithms.CalculateHeight(position, BiomMap[index].TemplateID, BiomMap[index].BiomCoefficient);
        }
    }
}