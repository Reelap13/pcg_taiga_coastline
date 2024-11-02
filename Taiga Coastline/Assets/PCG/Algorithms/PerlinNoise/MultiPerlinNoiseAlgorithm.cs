using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Algorithms.PerlinNoise
{
    [BurstCompile]
    public struct MultiPerlinNoiseAlgorithm : IHeightMapAlgorithm
    {
        public MultiPerlinNoiseAlgorithmData Data;
        public MultiPerlinNoiseAlgorithm(MultiPerlinNoiseAlgorithmData data) { this.Data = data; }

        public float CalculateHeightMap(float2 position, float coefficient)
        {
            float height = 0f;

            int id = 0;
            height += Data.Length > id++ ? Data.A1.CalculateHeightMap(position, coefficient) : 0f;
            height += Data.Length > id++ ? Data.A2.CalculateHeightMap(position, coefficient) : 0f;
            height += Data.Length > id++ ? Data.A3.CalculateHeightMap(position, coefficient) : 0f;
            height += Data.Length > id++ ? Data.A4.CalculateHeightMap(position, coefficient) : 0f;
            height += Data.Length > id++ ? Data.A5.CalculateHeightMap(position, coefficient) : 0f;
            height += Data.Length > id++ ? Data.A6.CalculateHeightMap(position, coefficient) : 0f;
            height += Data.Length > id++ ? Data.A7.CalculateHeightMap(position, coefficient) : 0f;

            return math.clamp(height, 0f, 1f);
        }
    }


    [BurstCompile]
    [System.Serializable]
    public struct MultiPerlinNoiseAlgorithmData
    {
        public int Length;
        public PerlinNoiseAlgorithm A1;
        public PerlinNoiseAlgorithm A2;
        public PerlinNoiseAlgorithm A3;
        public PerlinNoiseAlgorithm A4;
        public PerlinNoiseAlgorithm A5;
        public PerlinNoiseAlgorithm A6;
        public PerlinNoiseAlgorithm A7;

        public MultiPerlinNoiseAlgorithmData(MultiPerlinNoiseAlgorithmCreatorData data)
        {
            Length = data.AlgorithmsData.Length;
            int id = 0;
            A1 = Length > id++ ? new(data.AlgorithmsData[id - 1]) : new(); 
            A2 = Length > id++ ? new(data.AlgorithmsData[id - 1]) : new(); 
            A3 = Length > id++ ? new(data.AlgorithmsData[id - 1]) : new(); 
            A4 = Length > id++ ? new(data.AlgorithmsData[id - 1]) : new(); 
            A5 = Length > id++ ? new(data.AlgorithmsData[id - 1]) : new(); 
            A6 = Length > id++ ? new(data.AlgorithmsData[id - 1]) : new(); 
            A7 = Length > id++ ? new(data.AlgorithmsData[id - 1]) : new(); 
        }
    }

    // To get data from inspector
    [System.Serializable]
    public struct MultiPerlinNoiseAlgorithmCreatorData
    {
        public PerlinNoiseAlgorithmData[] AlgorithmsData;
    }
}