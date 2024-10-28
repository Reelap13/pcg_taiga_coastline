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
        
        public void AddAlgorithm(int biom_template_id, HeightsMapAlgorithmType type, IHeightsMapAlgorithm algorithm)
        {
            switch (type)
            {
                case HeightsMapAlgorithmType.PERLIN_NOISE:
                    PerlinNoiseAlgorithms.Add((PerlinNoiseAlgorithm)algorithm);
                    AlgorithmsData.Add(biom_template_id, 
                        new HeightsMapAlgorithmData(HeightsMapAlgorithmType.PERLIN_NOISE, PerlinNoiseAlgorithms.Length - 1));
                    break;
                case HeightsMapAlgorithmType.LINEAR:
                    LinearFunctions.Add((LinearFunction)algorithm);
                    AlgorithmsData.Add(biom_template_id,
                        new HeightsMapAlgorithmData(HeightsMapAlgorithmType.LINEAR, LinearFunctions.Length - 1));
                    break;
            }
        }

        public float CalculateHeight(float2 position, int biom_template_id)
        {
            HeightsMapAlgorithmData data = AlgorithmsData[biom_template_id];

            switch (data.Type)
            {
                case HeightsMapAlgorithmType.PERLIN_NOISE:
                    return PerlinNoiseAlgorithms[data.ID].CalculateHeight(position);
                case HeightsMapAlgorithmType.LINEAR:
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
        public HeightsMapAlgorithmType Type;
        public int ID;

        public HeightsMapAlgorithmData(HeightsMapAlgorithmType type, int ID)
        {
            this.Type = type;
            this.ID = ID;
        }
    }

}