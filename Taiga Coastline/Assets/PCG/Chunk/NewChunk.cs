using PCG_Map.Algorithms;
using PCG_Map.New_Bioms;
using PCG_Map.Objects;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Chunk
{
    public struct NewChunk
    {
        public float2 Position;
        public int Size;

        public int HTMapsResolution; // resolution of height and texture maps
        public int ObjectMapResolution; // resolution of object map

        public Map<PointBiom> HTBiomMap; // biom map for height and texture maps
        public Map<PointBiom> ObjectBiomMap; // biom map for objects

        public Map<float> HeightMap;

        public Map<int> TextureMap;
        public NativeParallelHashSet<int> Textures;

        public NativeList<ObjectData> Objects;

        public List<JobHandle> UnfinishedJobs;

        /*public NewChunk(float2 position, int size, int height_map_resolution, int texture_map_resolution, int object_map_resolution, int max_objects_number)
        {
            Position = position;
            Size = size;

            HeightMapResolution = height_map_resolution;
            HeightMap = new NativeArray<float>(height_map_resolution * height_map_resolution, Allocator.Persistent);
            HeightMapBioms = new NativeArray<PointBiom>(height_map_resolution * height_map_resolution, Allocator.Persistent);

            TextureMapResolution = texture_map_resolution;
            TextureMap = new NativeArray<int>(texture_map_resolution * texture_map_resolution, Allocator.Persistent);
            Textures = new NativeParallelHashSet<int>(10, Allocator.Persistent);
            TextureMapBioms = new NativeArray<PointBiom>(texture_map_resolution * texture_map_resolution, Allocator.Persistent);

            ObjectMapResolution = object_map_resolution;
            MaxObjectsNumber = max_objects_number;
            Objects = new NativeList<ObjectData>(max_objects_number * object_map_resolution * object_map_resolution, Allocator.Persistent);
            ObjectMapBioms = new NativeArray<PointBiom>(object_map_resolution * object_map_resolution, Allocator.Persistent);

            UnfinishedJobs = new List<JobHandle>();
        }*/

        public void Dispose()
        {
            HTBiomMap.Dispose();
            ObjectBiomMap.Dispose();

            HeightMap.Dispose();

            TextureMap.Dispose();
            Textures.Dispose();

            Objects.Dispose();
        }
    }
}