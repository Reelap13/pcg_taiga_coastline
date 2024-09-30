using PCG_Map.Heights;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace PCG_Map.Chunk
{
    public class ChunkGenerator : MonoBehaviour
    {
        [field: SerializeField]
        public ChunksFactory ChunksFactory { get; private set; }

        [SerializeField] private HeightsAgent _heights_agent;
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
                CalculateTextures(),
                CreateTerrain(),
                GenerateObjects());
            return chunk;
        }

        private Matrix2D CalculateHeights(Vector2 position)
        {
            Matrix2D map = new(HeightsMapResolution);
            map.FillWithPerlinNoise(100);
            return _heights_agent.GetHeights(position, Size, HeightsMapResolution);
        }

        private Matrix2D CalculateTextures()
        {
            return new(Size, 0);
        }

        private Terrain CreateTerrain()
        {
            return _terrain_creator.GenerateTerrain();
        }

        private List<GameObject> GenerateObjects()
        {
            return new();
        }
    }
}