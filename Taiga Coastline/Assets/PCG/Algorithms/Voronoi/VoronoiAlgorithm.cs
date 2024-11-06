using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace PCG_Map.Algorithms.Voronoi
{
    [BurstCompile]
    public struct VoronoiAlgorithm : IBiomsDistibutionsAlgorithm
    {
        [ReadOnly] public VoronoiAlgorithmData Data;

        public int GetBiom(float2 position)
        {
            return Calculate(position);
        }

        public int Calculate(float2 position)
        {
            int min_biom = -1;
            float min_distance = float.MaxValue;
            for (int i = 0; i < Data.Centers.Length; ++i)
            {
                float2 center = Data.Centers[i];
                float distance = math.distance(position, center);
                if (distance < min_distance)
                {
                    min_distance = distance;
                    min_biom = i;
                }
            }

            return min_biom;
        }

        public void Calculate2NearestCenters(float2 position, 
            out int first_nearest, out int second_nearest,
            out float first_nearest_distance, out float second_nearest_distance)
        {
            first_nearest = -1;
            second_nearest = -1;

            first_nearest_distance = float.MaxValue;
            second_nearest_distance = float.MaxValue;
            for (int i = 0; i < Data.Centers.Length; ++i)
            {
                float2 center = Data.Centers[i];
                float distance = math.distance(position, center);

                if (distance < first_nearest_distance)
                {
                    second_nearest = first_nearest;
                    second_nearest_distance = first_nearest_distance;

                    first_nearest = i;
                    first_nearest_distance = distance;
                }
                else if (distance < second_nearest_distance)
                {
                    second_nearest = i;
                    second_nearest_distance = distance;
                }
            }
        }

        public void Dispose()
        {
            Data.Dispose();
        }
    }

    [BurstCompile]
    [System.Serializable]
    public struct VoronoiAlgorithmData
    {
        [ReadOnly] public float4 Borders;
        [ReadOnly] public NativeArray<float2> Centers;

        public void Dispose()
        {
            Centers.Dispose();
        }
    }
}