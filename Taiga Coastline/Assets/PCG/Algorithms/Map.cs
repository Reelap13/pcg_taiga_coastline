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
            indexes = math.clamp(indexes, new(0), new(Size - 1));
            return GetData(indexes);
        }


        public T GetData(int2 indexes) { return GetData(indexes.x, indexes.y); }
        public T GetData(int i, int j) { return Data[i * Size + j]; }
        public T GetData(int index) { return Data[index]; }

        public void SetData(T value, int i, int j) { Data[i * Size + j] = value; }
        public void SetData(T value, int index) { Data[index] = value; }

        public bool IsInMap(int i, int j) { return !(i < 0 || i >= Size || j < 0 || j >= Size); }

        public void Dispose()
        {
            Data.Dispose();
        }
    }
}