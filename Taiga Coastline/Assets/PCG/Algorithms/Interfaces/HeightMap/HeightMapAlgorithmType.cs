using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Algorithms
{
    public enum HeightMapAlgorithmType
    {
        PERLIN_NOISE = AlgorithmType.PERLIN_NOISE,
        MULTI_PERLIN_NOISE = AlgorithmType.MULTI_PERLIN_NOISE,
        LINEAR = AlgorithmType.LINEAR
    }
}