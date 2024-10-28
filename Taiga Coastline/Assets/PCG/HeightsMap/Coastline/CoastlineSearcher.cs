using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine;

namespace PCG_Map.Heights
{
    public class CoastlineSearcher
    {
        private NativeArray<Vector2> _points;

        public CoastlineSearcher(NativeArray<Vector2> points)
        {
            _points = points;
        }

        public NativeArray<float> GetYValues(NativeArray<float> x_values)
        {
            NativeArray<float> y_values = new NativeArray<float>(x_values.Length, Allocator.TempJob);

            var job = new FindYValuesJob
            {
                Points = _points,
                XValues = x_values,
                YValues = y_values
            };

            JobHandle jobHandle = job.Schedule(x_values.Length, 64);
            jobHandle.Complete();

            return y_values;
        }

        [BurstCompile]
        public struct FindYValuesJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<Vector2> Points;
            [ReadOnly] public NativeArray<float> XValues;
            public NativeArray<float> YValues;

            public void Execute(int index)
            {
                float x = XValues[index];
                YValues[index] = FindYValueByBinarySearch(x);
            }

            private float FindYValueByBinarySearch(float x)
            {
                int left = 0;
                int right = Points.Length - 1;

                while (left < right)
                {
                    int middle = (left + right) / 2;

                    if (Points[middle].x < x)
                    {
                        left = middle + 1;
                    }
                    else
                    {
                        right = middle;
                    }
                }

                if (left == 0) return Points[0].y;
                if (left == Points.Length) return Points[Points.Length - 1].y;

                Vector2 p1 = Points[left - 1];
                Vector2 p2 = Points[left];

                float t = (x - p1.x) / (p2.x - p1.x);
                return Mathf.Lerp(p1.y, p2.y, t);
            }
        }
    }
}
