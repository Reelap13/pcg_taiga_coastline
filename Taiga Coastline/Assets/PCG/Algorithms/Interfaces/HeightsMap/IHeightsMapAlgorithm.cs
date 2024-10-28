using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Algorithms
{
    public interface IHeightsMapAlgorithm
    {
        public float CalculateHeight(float2 position);
    }
}