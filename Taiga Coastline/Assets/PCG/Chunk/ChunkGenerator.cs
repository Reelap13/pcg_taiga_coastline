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

        public NewChunk GenerateChunk(Vector2 default_position)
        {
            float2 position = default_position;
            NewChunk chunk = new NewChunk(position, Size, HeightMapResolution, HeightMapResolution);
            chunk.UnfinishedJobs.Add(CalculateHeightMap(chunk));

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
            int size = HeightMapResolution;
            float step = (float)chunk.Size / (chunk.HeightMapResolution - 1);
            NativeArray<int> biom_id = chunk.HeightsMapBiomsID;
            NativeArray<float> height_map = chunk.HeightMap;


            JobHandle bioms_id_handle = BiomsController.GetBioms(position, size, step, biom_id).Schedule(size * size, 64);

            JobHandle height_map_handle = HeightsAgent.GetHeights(position, size, step, biom_id, height_map).Schedule(size * size, 64, bioms_id_handle);

            return height_map_handle;
        }

        private ChunkTexturesData CalculateTextures(Vector2 position)
        {
            ChunkTexturesData textures_data = new ChunkTexturesData(new(Size, Size));

            _textures_agent.SetTextures(position, new(Size, Size), textures_data);

            return textures_data;
        }

        private Terrain CreateTerrain()
        {
            return _terrain_creator.GenerateTerrain();
        }

        private List<GameObject> GenerateObjects(Vector2 position)
        {
            return ObjectsAgent.Instance.GetObjects(position, new(Size, Size));
        }
    }
}