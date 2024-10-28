using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Algorithms.Linear
{
    [BurstCompile]
    public struct LinearFunction : IHeightsMapAlgorithm
    {
        public LinearFunctionData Data;
        public LinearFunction(LinearFunctionData data) { this.Data = data; }

        public float CalculateHeight(float2 position)
        {
            return Calculate(position.x, position.y);
        }

        public float Calculate(float x, float y)
        {
            float value = Data.BaseValue + 
                (x + Data.Offsets.x) * Data.Coefficients.x + 
                (y + Data.Offsets.y) * Data.Coefficients.y;

            value = math.clamp(value, Data.Borders[0], Data.Borders[1]);

            return math.lerp(0, 1, value / Data.Borders[1]);
        }
    }

    [BurstCompile]
    [System.Serializable]
    public struct LinearFunctionData
    {
        public float BaseValue;
        public float2 Coefficients;
        public float2 Offsets;
        public float2 Borders;

        public LinearFunctionData(LinearFunctionCreatorData data)
        {
            BaseValue = data.BaseValue;
            Coefficients = data.Coefficients;
            Offsets = data.Offsets;
            Borders = data.Borders;
        }
    }

    // To get data from inspector
    [System.Serializable]
    public struct LinearFunctionCreatorData
    {
        public float BaseValue;
        public Vector2 Coefficients;
        public Vector2 Offsets;
        public Vector2 Borders;
    }
}