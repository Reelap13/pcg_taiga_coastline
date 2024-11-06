using PCG_Map.Algorithms.Linear;
using PCG_Map.Algorithms.PerlinNoise;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace PCG_Map.Algorithms
{
    public struct AlgorithmsData
    {
        public NativeHashMap<int, PerlinNoiseAlgorithm> PerlinNoiseAlgorithms;
        public NativeHashMap<int, LinearFunction> LinearFunctions;

        public AlgorithmsData(int capacity)
        {
            PerlinNoiseAlgorithms = new NativeHashMap<int, PerlinNoiseAlgorithm>(capacity, Allocator.Persistent);
            LinearFunctions = new NativeHashMap<int, LinearFunction>(capacity, Allocator.Persistent);
        }

        public void Dispose()
        {
            if (PerlinNoiseAlgorithms.IsCreated) PerlinNoiseAlgorithms.Dispose();
            if (LinearFunctions.IsCreated) LinearFunctions.Dispose();
        }
    }
}