using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Chunk
{
    public class NewChunk
    {
        public float2 Position;
        public int Size;
        public int HeightMapResolution;
        public int TextureMapResolution;
        public NativeArray<float> HeightMap;
        public NativeArray<int> HeightsMapBiomsID;
        public NativeArray<int> AlphaMap;
        public List<JobHandle> UnfinishedJobs;

        public NewChunk(float2 position, int size, int height_map_resolution, int texture_map_resolution)
        {
            Position = position;
            Size = size;
            HeightMapResolution = height_map_resolution;
            TextureMapResolution = texture_map_resolution;
            HeightMap = new NativeArray<float>(height_map_resolution * height_map_resolution, Allocator.Persistent);
            HeightsMapBiomsID = new NativeArray<int>(height_map_resolution * height_map_resolution, Allocator.Persistent);
            AlphaMap = new NativeArray<int>(texture_map_resolution * texture_map_resolution, Allocator.Persistent);
            UnfinishedJobs = new List<JobHandle>();
        }

        public void Dispose()
        {
            HeightMap.Dispose();
            HeightsMapBiomsID.Dispose();
            AlphaMap.Dispose();
        }
    }
}