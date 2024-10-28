using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace PCG_Map.Algorithms.PerlinNoise
{
    [BurstCompile]
    public struct PerlinNoiseAlgorithm : IHeightsMapAlgorithm
    {
        public PerlinNoiseAlgorithmData Data;
        public PerlinNoiseAlgorithm(PerlinNoiseAlgorithmData data) { this.Data = data; }

        public float CalculateHeight(float2 position)
        {
            return Calculate(position.x, position.y);
        }

        public float Calculate(float x, float y)
        {
            float2 position = new float2(x / Data.Smoothness, y / Data.Smoothness);

            //useing a Simplex Nise insted a Perlin Noise as don't have a perline noise implementation(fixed in the future)
            float height = Data.BaseValue + noise.snoise(position) * Data.Amplitude;
            height = math.clamp(height, Data.Borders.x, Data.Borders.y);
            return math.lerp(0, 1, height / Data.Borders.y);
        }
    }

    [BurstCompile]
    [System.Serializable]
    public struct PerlinNoiseAlgorithmData
    {
        public float BaseValue;
        public float Smoothness;
        public float Amplitude;
        public float2 Borders;

        public PerlinNoiseAlgorithmData(PerlinNoiseAlgorithmCreatorData data)
        {
            BaseValue = data.BaseValue;
            Smoothness = data.Smoothness;
            Amplitude = data.Amplitude;
            Borders = data.Borders;
        }
    }

    // To get data from inspector
    [System.Serializable]
    public struct PerlinNoiseAlgorithmCreatorData
    {
        public float BaseValue;
        public float Smoothness;
        public float Amplitude;
        public Vector2 Borders;
    }
}