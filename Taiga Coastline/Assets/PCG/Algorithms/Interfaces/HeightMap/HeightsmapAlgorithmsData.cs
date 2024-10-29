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
    public struct HeightsmapAlgorithmsData
    {
        public NativeHashMap<int, HeightsMapAlgorithmData> AlgorithmsData;
        public NativeList<PerlinNoiseAlgorithm> PerlinNoiseAlgorithms;
        public NativeList<LinearFunction> LinearFunctions;
        
        public void AddAlgorithm(int biom_template_id, HeightMapAlgorithmType type, IHeightsMapAlgorithm algorithm)
        {
            switch (type)
            {
                case HeightMapAlgorithmType.PERLIN_NOISE:
                    PerlinNoiseAlgorithms.Add((PerlinNoiseAlgorithm)algorithm);
                    AlgorithmsData.Add(biom_template_id, 
                        new HeightsMapAlgorithmData(HeightMapAlgorithmType.PERLIN_NOISE, PerlinNoiseAlgorithms.Length - 1));
                    break;
                case HeightMapAlgorithmType.LINEAR:
                    LinearFunctions.Add((LinearFunction)algorithm);
                    AlgorithmsData.Add(biom_template_id,
                        new HeightsMapAlgorithmData(HeightMapAlgorithmType.LINEAR, LinearFunctions.Length - 1));
                    break;
            }
        }

        public float CalculateHeight(float2 position, int biom_template_id)
        {
            HeightsMapAlgorithmData data = AlgorithmsData[biom_template_id];

            switch (data.Type)
            {
                case HeightMapAlgorithmType.PERLIN_NOISE:
                    return PerlinNoiseAlgorithms[data.ID].CalculateHeight(position);
                case HeightMapAlgorithmType.LINEAR:
                    return LinearFunctions[data.ID].CalculateHeight(position);
            }

            return -1;
        }

        public void Dispose()
        {
            AlgorithmsData.Dispose();
            PerlinNoiseAlgorithms.Dispose();
            LinearFunctions.Dispose();
        }
    }

    [BurstCompile]
    [System.Serializable]
    public struct HeightsMapAlgorithmData
    {
        public HeightMapAlgorithmType Type;
        public int ID;

        public HeightsMapAlgorithmData(HeightMapAlgorithmType type, int ID)
        {
            this.Type = type;
            this.ID = ID;
        }
    }

}