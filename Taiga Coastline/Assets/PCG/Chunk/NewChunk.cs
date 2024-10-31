using PCG_Map.Objects;
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
        public NativeArray<float> HeightMap;
        public NativeArray<int> HeightMapBiomsID;

        public int TextureMapResolution;
        public NativeArray<int> TextureMap;
        public NativeParallelHashSet<int> Textures;
        public NativeArray<int> TextureMapBiomsID;

        public int ObjectMapResolution;
        public int MaxObjectsNumber;
        public NativeList<ObjectData> Objects;
        public NativeArray<int> ObjectsBiomsID;

        public List<JobHandle> UnfinishedJobs;

        public NewChunk(float2 position, int size, int height_map_resolution, int texture_map_resolution, int object_map_resolution, int max_objects_number)
        {
            Position = position;
            Size = size;

            HeightMapResolution = height_map_resolution;
            HeightMap = new NativeArray<float>(height_map_resolution * height_map_resolution, Allocator.Persistent);
            HeightMapBiomsID = new NativeArray<int>(height_map_resolution * height_map_resolution, Allocator.Persistent);

            TextureMapResolution = texture_map_resolution;
            TextureMap = new NativeArray<int>(texture_map_resolution * texture_map_resolution, Allocator.Persistent);
            Textures = new NativeParallelHashSet<int>(10, Allocator.Persistent);
            TextureMapBiomsID = new NativeArray<int>(texture_map_resolution * texture_map_resolution, Allocator.Persistent);

            ObjectMapResolution = object_map_resolution;
            MaxObjectsNumber = max_objects_number;
            Objects = new NativeList<ObjectData>(max_objects_number * object_map_resolution * object_map_resolution, Allocator.Persistent);
            ObjectsBiomsID = new NativeArray<int>(object_map_resolution * object_map_resolution, Allocator.Persistent);

            UnfinishedJobs = new List<JobHandle>();
        }

        public void Dispose()
        {
            HeightMap.Dispose();
            HeightMapBiomsID.Dispose();

            TextureMap.Dispose();
            Textures.Dispose();
            TextureMapBiomsID.Dispose();

            Objects.Dispose();
            ObjectsBiomsID.Dispose();
        }
    }
}