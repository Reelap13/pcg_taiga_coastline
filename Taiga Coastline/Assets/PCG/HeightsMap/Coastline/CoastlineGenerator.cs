using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics; // Подключите эту библиотеку

namespace PCG_Map.Heights
{
    public class CoastlineGenerator : MonoBehaviour
    {
        [SerializeField] private int _iteration_number = 5;
        [SerializeField] private float _height_offset = 1f;
        [SerializeField] private float _roughness = 0.5f;
        [SerializeField] private Vector2 _x_boarders = new(-1000, 1000);
        [SerializeField] private float _y_start = 0;
        [SerializeField] private int _seed = 123;

        private NativeArray<Vector2> _points;

        public void Initialize()
        {
            GenerateCoastline();
        }

        private void GenerateCoastline()
        {
            NativeArray<Vector2> initialPoints = new NativeArray<Vector2>(2, Allocator.TempJob);
            initialPoints[0] = new Vector2(_x_boarders.x, _y_start);
            initialPoints[1] = new Vector2(_x_boarders.y, _y_start);

            var job = new GenerateCoastlineJob
            {
                IterationNumber = _iteration_number,
                HeightOffset = _height_offset,
                Roughness = _roughness,
                Points = initialPoints,
                Seed = _seed
            };

            JobHandle jobHandle = job.Schedule();
            jobHandle.Complete();

            _points = new NativeArray<Vector2>(job.NewPointCount, Allocator.Persistent);
            job.NewPoints.CopyTo(_points);

            initialPoints.Dispose();
        }

        [BurstCompile]
        public struct GenerateCoastlineJob : IJob
        {
            public int IterationNumber;
            public float HeightOffset;
            public float Roughness;
            public int Seed;   

            [ReadOnly] public NativeArray<Vector2> Points;
            public NativeArray<Vector2> NewPoints;
            public int NewPointCount;

            public void Execute()
            {
                Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)Seed);

                NewPointCount = Points.Length;

                for (int iteration = 0; iteration < IterationNumber; ++iteration)
                {
                    NativeArray<Vector2> temp_new_points = new NativeArray<Vector2>(NewPointCount * 2 - 1, Allocator.Temp);
                    int temp_index = 0;

                    for (int i = 0; i < NewPointCount - 1; ++i)
                    {
                        Vector2 start = NewPoints[i];
                        Vector2 end = NewPoints[i + 1];
                        temp_new_points[temp_index++] = start;

                        Vector2 middle = (start + end) / 2;
                        middle.y += random.NextFloat(-HeightOffset, HeightOffset) * math.pow(Roughness, iteration);
                        temp_new_points[temp_index++] = middle;
                    }

                    temp_new_points[temp_index] = NewPoints[NewPointCount - 1];
                    NewPointCount = temp_index + 1;

                    NewPoints.Dispose();
                    NewPoints = new NativeArray<Vector2>(NewPointCount, Allocator.Temp);
                    temp_new_points.CopyTo(NewPoints);

                    temp_new_points.Dispose();
                }
            }
        }

        private void OnDestroy()
        {
            if (_points.IsCreated)
                _points.Dispose();
        }

        public NativeArray<Vector2> GetPoints() => _points;
    }
}
