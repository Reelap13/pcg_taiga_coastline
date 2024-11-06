using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Algorithms
{
    public interface IHeightMapAlgorithm
    {
        public float CalculateHeightMap(float2 position, float coefficient);
    }
}