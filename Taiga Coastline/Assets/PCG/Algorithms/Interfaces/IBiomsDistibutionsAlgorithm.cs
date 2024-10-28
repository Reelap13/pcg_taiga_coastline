using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Algorithms
{
    public interface IBiomsDistibutionsAlgorithm
    {
        public int GetBiom(float2 position);
    }
}