using PCG_Map.Heights;
using PCG_Map.Objects;
using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG_Map.Chunk
{
    public class ChunkGenerator : MonoBehaviour
    {
        [field: SerializeField]
        public ChunksFactory ChunksFactory { get; private set; }

        [SerializeField] private HeightsAgent _heights_agent;
        [SerializeField] private TexturesAgent _textures_agent;
        [SerializeField] private TerrainCreator _terrain_creator;

        public Vector2Int Size => ChunksFactory.Size;
        public int HeightsMapResolution => ChunksFactory.HeightsMapResolution;

        public Chunk GenerateChunk(Vector2 position)
        {
            Chunk chunk = new Chunk(
                new($"Chunk {position.x} {position.y}"),
                position,
                Size,
                HeightsMapResolution,
                CalculateHeights(position),
                CalculateTextures(position),
                CreateTerrain(),
                GenerateObjects(position));
            return chunk;
        }

        private Matrix2D CalculateHeights(Vector2 position)
        {
            return _heights_agent.GetHeights(position, Size, HeightsMapResolution);
        }

        private ChunkTexturesData CalculateTextures(Vector2 position)
        {
            ChunkTexturesData textures_data = new ChunkTexturesData(Size);

            _textures_agent.SetTextures(position, Size, textures_data);

            return textures_data;
        }

        private Terrain CreateTerrain()
        {
            return _terrain_creator.GenerateTerrain();
        }

        private List<GameObject> GenerateObjects(Vector2 position)
        {
            return ObjectsAgent.Instance.GetObjects(position, Size);
        }
    }
}