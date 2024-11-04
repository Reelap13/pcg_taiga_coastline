using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Algorithms
{
    public struct Map<T> where T : struct
    {
        public float2 Position;
        public int Size;
        public float Step;
        public NativeArray<T> Data;

        public T GetData(float2 position)
        {
            float2 shift = position - Position;
            int2 indexes = new(math.round(shift / Step));

            return GetData(indexes);
        }


        public T GetData(int2 indexes) { return GetData(indexes.x, indexes.y); }
        public T GetData(int i, int j) { return Data[i * Size + j]; }
        public T GetData(int index) { return Data[index]; }

        public void Dispose()
        {
            Data.Dispose();
        }
    }
}