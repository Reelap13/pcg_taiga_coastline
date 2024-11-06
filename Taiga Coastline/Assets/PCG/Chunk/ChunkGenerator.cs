using PCG_Map.New_Bioms;
using PCG_Map.Heights;
using PCG_Map.Objects;
using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Jobs;
using PCG_Map.Algorithms;

namespace PCG_Map.Chunk
{
    public class ChunkGenerator : MonoBehaviour
    {
        [field: SerializeField]
        public ChunksFactory ChunksFactory { get; private set; }

        [SerializeField] private TexturesAgent _textures_agent;
        [SerializeField] private TerrainCreator _terrain_creator;
        [SerializeField] private int _object_number_per_field = 2;

        public int Size => ChunksFactory.Size;
        public int HTMapResolution => ChunksFactory.HTMapResolution;
        public int ObjectMapResolution => ChunksFactory.ObjectMapResolution;

        public BiomsController BiomsController => BiomsController.Instance;
        public HeightsAgent HeightsAgent => HeightsAgent.Instance;
        public TexturesAgent TexturesAgent => TexturesAgent.Instance;
        public ObjectsAgent ObjectsAgent => ObjectsAgent.Instance;

        public NewChunk GenerateChunk(Vector2 default_position)
        {
            float2 position = default_position;
            NewChunk chunk = InitializeChunk(position);

            List<JobHandle> biom_map_handles = CalculateBiomMaps(chunk);

            JobHandle height_map_handle = CalculateHeightMap(chunk, biom_map_handles[0]);
            JobHandle texture_map_handle = CalculateTextureMap(chunk, height_map_handle);
            JobHandle objects_handle = GenerateObjects(chunk, biom_map_handles[1], texture_map_handle);

            chunk.UnfinishedJobs.AddRange(biom_map_handles);
            chunk.UnfinishedJobs.AddRange(new List<JobHandle>(){
                height_map_handle,
                texture_map_handle,
                objects_handle
            });

            return chunk;
            /*return new Chunk(
                new($"Chunk {position.x} {position.y}"),
                position,
                Size,
                HeightMapResolution,
                CalculateHeights(position),
                null,
                CreateTerrain(),
                new()); */
            /*Chunk chunk = new Chunk(
                new($"Chunk {position.x} {position.y}"),
                position,
                Size,
                HeightsMapResolution,
                CalculateHeights(position),
                CalculateTextures(position),
                CreateTerrain(),
                GenerateObjects(position));
            return chunk;*/
        }

        private NewChunk InitializeChunk(float2 position)
        {
            int ht_size = HTMapResolution;
            float ht_step = (float)Size / (ht_size - 1);

            int objects_size = ObjectMapResolution;
            float objects_step = (float)Size / objects_size;

            NativeArray<PointBiom> ht_biom_map_data = new NativeArray<PointBiom>(ht_size * ht_size, Allocator.Persistent);
            Map<PointBiom> ht_biom_map = new Map<PointBiom>()
            {
                Position = position,
                Size = ht_size,
                Step = ht_step,
                Data = ht_biom_map_data
            };
            NativeArray<PointBiom> object_biom_map_data = new NativeArray<PointBiom>(objects_size * objects_size, Allocator.Persistent);
            Map<PointBiom> object_biom_map = new Map<PointBiom>()
            {
                Position = position,
                Size = objects_size,
                Step = objects_step,
                Data = object_biom_map_data
            };

            NativeArray<float> height_map_data = new NativeArray<float>(ht_size * ht_size, Allocator.Persistent);
            Map<float> height_map = new Map<float>()
            {
                Position = position,
                Size = ht_size,
                Step = ht_step,
                Data = height_map_data
            };

            NativeArray<int> texture_map_data = new NativeArray<int>(ht_size * ht_size, Allocator.Persistent);
            Map<int> texture_map = new Map<int>()
            {
                Position = position,
                Size = ht_size,
                Step = ht_step,
                Data = texture_map_data
            };
            NativeParallelHashSet<int> textures = new NativeParallelHashSet<int>(10, Allocator.Persistent); // 10 - max number of textures in one bioms(magic number)

            NativeList<ObjectData> objects = new NativeList<ObjectData>(_object_number_per_field * objects_size * objects_size, Allocator.Persistent);

            List<JobHandle> unfinished_jobs = new List<JobHandle>();

            NewChunk chunk = new NewChunk
            {
                Position = position,
                Size = Size,

                HTMapsResolution = HTMapResolution,
                ObjectMapResolution = ObjectMapResolution,

                HTBiomMap = ht_biom_map,
                ObjectBiomMap = object_biom_map,

                HeightMap = height_map,

                TextureMap = texture_map,
                Textures = textures,

                Objects = objects,

                UnfinishedJobs = unfinished_jobs
            };

            return chunk;
        }

        private List<JobHandle> CalculateBiomMaps(NewChunk chunk)
        {
            JobHandle ht_biom_maps_hendle = BiomsController.GetBioms(
                chunk.HTBiomMap.Position, 
                chunk.HTBiomMap.Size, 
                chunk.HTBiomMap.Step, 
                chunk.HTBiomMap.Data).Schedule(chunk.HTBiomMap.Size * chunk.HTBiomMap.Size, 64);

            JobHandle object_biom_maps_hendle = BiomsController.GetBioms(
                chunk.ObjectBiomMap.Position,
                chunk.ObjectBiomMap.Size,
                chunk.ObjectBiomMap.Step,
                chunk.ObjectBiomMap.Data).Schedule(chunk.ObjectBiomMap.Size * chunk.ObjectBiomMap.Size, 64);

            return new() { ht_biom_maps_hendle, object_biom_maps_hendle };
        }

        private JobHandle CalculateHeightMap(NewChunk chunk, JobHandle biom_map_handle)
        {
            JobHandle height_map_handle = HeightsAgent.GetHeightMap(
                chunk.HeightMap.Position,
                chunk.HeightMap.Size,
                chunk.HeightMap.Step, 
                chunk.HTBiomMap.Data,
                chunk.HeightMap.Data).Schedule(chunk.HeightMap.Size * chunk.HeightMap.Size, 64, biom_map_handle);

            return height_map_handle;
        }

        private JobHandle CalculateTextureMap(NewChunk chunk, JobHandle height_map_handle)
        {
            JobHandle texture_map_handle = TexturesAgent.GetTextureMap(
                chunk.TextureMap.Position,
                chunk.TextureMap.Size,
                chunk.TextureMap.Step, 
                chunk.HTBiomMap.Data,
                chunk.TextureMap.Data, 
                chunk.Textures,
                chunk.HeightMap).Schedule(chunk.TextureMap.Size * chunk.TextureMap.Size, 64, height_map_handle);

            return texture_map_handle;
        }

        private JobHandle GenerateObjects(NewChunk chunk, JobHandle biom_map_handle, JobHandle texture_map_handle)
        {
            float2 position = chunk.Position;
            int size = chunk.ObjectMapResolution;
            float step = (float)chunk.Size / size;
            NativeArray<PointBiom> biom_map = chunk.ObjectBiomMap.Data;
            NativeList<ObjectData> objects = chunk.Objects;

            JobHandle combined_handle = JobHandle.CombineDependencies(biom_map_handle, texture_map_handle);

            JobHandle objects_handle = ObjectsAgent.GetObjects(
                position, 
                size,
                step, 
                biom_map,
                objects,
                chunk.HeightMap).Schedule(combined_handle);

            return objects_handle;
        }
    }
}