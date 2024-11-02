using PCG_Map.Algorithms.Linear;
using PCG_Map.Algorithms.PerlinNoise;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Algorithms
{
    [BurstCompile]
    public struct HeightMapAlgorithmsData
    {
        public NativeHashMap<int, HeightMapAlgorithmData> AlgorithmsData;
        public NativeList<PerlinNoiseAlgorithm> PerlinNoiseAlgorithms;
        public NativeList<MultiPerlinNoiseAlgorithm> MultiPerlinNoiseAlgorithms;
        public NativeList<LinearFunction> LinearFunctions;
        
        public void AddAlgorithm(int biom_template_id, HeightMapAlgorithmType type, IHeightMapAlgorithm algorithm)
        {
            switch (type)
            {
                case HeightMapAlgorithmType.PERLIN_NOISE:
                    PerlinNoiseAlgorithms.Add((PerlinNoiseAlgorithm)algorithm);
                    AlgorithmsData.Add(biom_template_id, 
                        new HeightMapAlgorithmData(HeightMapAlgorithmType.PERLIN_NOISE, PerlinNoiseAlgorithms.Length - 1));
                    break;
                case HeightMapAlgorithmType.MULTI_PERLIN_NOISE:
                    MultiPerlinNoiseAlgorithms.Add((MultiPerlinNoiseAlgorithm)algorithm);
                    AlgorithmsData.Add(biom_template_id,
                        new HeightMapAlgorithmData(HeightMapAlgorithmType.MULTI_PERLIN_NOISE, MultiPerlinNoiseAlgorithms.Length - 1));
                    break;
                case HeightMapAlgorithmType.LINEAR:
                    LinearFunctions.Add((LinearFunction)algorithm);
                    AlgorithmsData.Add(biom_template_id,
                        new HeightMapAlgorithmData(HeightMapAlgorithmType.LINEAR, LinearFunctions.Length - 1));
                    break;
            }
        }

        public float CalculateHeight(float2 position, int biom_template_id, float coefficient)
        {
            HeightMapAlgorithmData data = AlgorithmsData[biom_template_id];

            switch (data.Type)
            {
                case HeightMapAlgorithmType.PERLIN_NOISE:
                    return PerlinNoiseAlgorithms[data.ID].CalculateHeightMap(position, coefficient);
                case HeightMapAlgorithmType.MULTI_PERLIN_NOISE:
                    return MultiPerlinNoiseAlgorithms[data.ID].CalculateHeightMap(position, coefficient);
                case HeightMapAlgorithmType.LINEAR:
                    return LinearFunctions[data.ID].CalculateHeightMap(position, coefficient);
            }

            return -1;
        }

        public void Dispose()
        {
            AlgorithmsData.Dispose();
            PerlinNoiseAlgorithms.Dispose();
            MultiPerlinNoiseAlgorithms.Dispose();
            LinearFunctions.Dispose();
        }
    }

    [BurstCompile]
    [System.Serializable]
    public struct HeightMapAlgorithmData
    {
        public HeightMapAlgorithmType Type;
        public int ID;

        public HeightMapAlgorithmData(HeightMapAlgorithmType type, int ID)
        {
            this.Type = type;
            this.ID = ID;
        }
    }

}