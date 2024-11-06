using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace PCG_Map.Algorithms.PerlinNoise
{
    [BurstCompile]
    public struct PerlinNoiseAlgorithm : IHeightMapAlgorithm
    {
        public PerlinNoiseAlgorithmData Data;
        public PerlinNoiseAlgorithm(PerlinNoiseAlgorithmData data) { this.Data = data; }

        public float CalculateHeightMap(float2 position, float coefficient)
        {
            return Calculate(position.x, position.y, coefficient);
        }

        public float Calculate(float x, float y, float coefficient)
        {
            float2 position = new float2(x / Data.Smoothness, y / Data.Smoothness);

            //useing a Simplex Nise insted a Perlin Noise as don't have a perline noise implementation(fixed in the future)
            float noise_value = (noise.snoise(position) + 1) / 2f;
            float height = Data.BaseValue + noise_value * Data.Amplitude * ProcessCoefficient(coefficient);
            height = math.clamp(height, Data.Borders.x, Data.Borders.y);
            return math.lerp(0, 1, height / Data.Borders.y);
        }

        private float ProcessCoefficient(float coefficient)
        {
            coefficient = math.clamp(coefficient, Data.CoefficientBorders.x, Data.CoefficientBorders.y);    
            switch (Data.CoefficientEffect)
            {
                case CoefficientEffectType.IGNORE: return 1.0f;
                case CoefficientEffectType.DIRECT: return coefficient * Data.Coefficient;
                case CoefficientEffectType.ADDITIONAL: return 1.0f + coefficient * Data.Coefficient;
            }

            return 1.0f;
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
        public CoefficientEffectType CoefficientEffect;
        public float Coefficient;
        public float2 CoefficientBorders;

        public PerlinNoiseAlgorithmData(PerlinNoiseAlgorithmCreatorData data)
        {
            BaseValue = data.BaseValue;
            Smoothness = data.Smoothness;
            Amplitude = data.Amplitude;
            Borders = data.Borders;
            CoefficientEffect = data.CoefficientEffect;
            Coefficient = data.Coefficient;
            CoefficientBorders = data.CoefficientBorders;
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
        public CoefficientEffectType CoefficientEffect;
        public float Coefficient;
        public Vector2 CoefficientBorders;
    }
}