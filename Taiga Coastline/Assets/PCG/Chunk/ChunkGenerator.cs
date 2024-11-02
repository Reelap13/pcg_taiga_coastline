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

namespace PCG_Map.Chunk
{
    public class ChunkGenerator : MonoBehaviour
    {
        [field: SerializeField]
        public ChunksFactory ChunksFactory { get; private set; }

        [SerializeField] private TexturesAgent _textures_agent;
        [SerializeField] private TerrainCreator _terrain_creator;

        public int Size => ChunksFactory.Size;
        public int HeightMapResolution => ChunksFactory.HeightMapResolution;

        public BiomsController BiomsController => BiomsController.Instance;
        public HeightsAgent HeightsAgent => HeightsAgent.Instance;
        public TexturesAgent TexturesAgent => TexturesAgent.Instance;
        public ObjectsAgent ObjectsAgent => ObjectsAgent.Instance;

        public NewChunk GenerateChunk(Vector2 default_position)
        {
            float2 position = default_position;
            NewChunk chunk = new NewChunk(position, Size, HeightMapResolution, HeightMapResolution, Size, 4);
            chunk.UnfinishedJobs.Add(CalculateHeightMap(chunk));
            chunk.UnfinishedJobs.Add(CalculateTextureMap(chunk));
            chunk.UnfinishedJobs.Add(GenerateObjects(chunk));

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

        private JobHandle CalculateHeightMap(NewChunk chunk)
        {
            float2 position = chunk.Position;
            int size = chunk.HeightMapResolution;
            float step = (float)chunk.Size / (chunk.HeightMapResolution - 1);
            NativeArray<PointBiom> biom_map = chunk.HeightMapBioms;
            NativeArray<float> height_map = chunk.HeightMap;


            JobHandle bioms_id_handle = BiomsController.GetBioms(position, size, step, biom_map).Schedule(size * size, 64);

            JobHandle height_map_handle = HeightsAgent.GetHeightMap(position, size, step, biom_map, height_map).Schedule(size * size, 64, bioms_id_handle);

            return height_map_handle;
        }

        private JobHandle CalculateTextureMap(NewChunk chunk)
        {
            float2 position = chunk.Position;
            int size = chunk.TextureMapResolution;
            float step = (float)chunk.Size / (chunk.TextureMapResolution - 1);
            NativeArray<PointBiom> biom_map = chunk.TextureMapBioms;
            NativeArray<int> texture_map = chunk.TextureMap;
            NativeParallelHashSet<int> textures = chunk.Textures;

            JobHandle bioms_id_handle = BiomsController.GetBioms(position, size, step, biom_map).Schedule(size * size, 64);

            JobHandle texture_map_handle = TexturesAgent.GetTextureMap(position, size, step, biom_map, texture_map, textures).Schedule(size * size, 64, bioms_id_handle);

            return texture_map_handle;
        }

        private JobHandle GenerateObjects(NewChunk chunk)
        {
            float2 position = chunk.Position;
            int size = chunk.ObjectMapResolution;
            float step = (float)chunk.Size / size;
            NativeArray<PointBiom> biom_map = chunk.ObjectMapBioms;
            NativeList<ObjectData> objects = chunk.Objects;

            JobHandle bioms_id_handle = BiomsController.GetBioms(position, size, step, biom_map).Schedule(size * size, 64);

            JobHandle objects_handle = ObjectsAgent.GetObjects(position, size, step, biom_map, objects).Schedule(bioms_id_handle);

            return objects_handle;
        }
    }
}