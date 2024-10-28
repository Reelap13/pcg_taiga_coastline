using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG_Map.Chunk
{
    public class NewChunkApplier : MonoBehaviour
    {
        [SerializeField] private TerrainCreator _terrain_creator;

        public ChunkData ApplyToChunk(NewChunk chunk)
        {
            GameObject chunk_obj = new GameObject($"Chunk {chunk.Position}");

            Terrain terrain = CreateTerrain(chunk_obj, chunk);

            ApplyPosition(chunk_obj, chunk);
            ApplyHeights(chunk, terrain);

            ChunkData data = new ChunkData()
            {
                Obj = chunk_obj,
                Position = chunk.Position,
                Size = chunk.Size,
                Terrain = terrain
            };
            chunk.Dispose();

            return data;
        }

        private Terrain CreateTerrain(GameObject chunk_obj, NewChunk chunk)
        {
            Terrain terrain = _terrain_creator.GenerateTerrain();
            terrain.transform.SetParent(chunk_obj.transform);

            terrain.terrainData.heightmapResolution = chunk.HeightMapResolution;
            terrain.terrainData.size = new Vector3(chunk.Size, Generator.Instance.HeightsAgent.TerrainHeight, chunk.Size);

            return terrain;
        }


        private void ApplyPosition(GameObject chunk_obj, NewChunk chunk)
        {
            chunk_obj.transform.position = new Vector3(chunk.Position.x, 0, chunk.Position.y);
        }

        private void ApplyHeights(NewChunk chunk, Terrain terrain)
        {
            float[,] height_map = new float[chunk.HeightMapResolution, chunk.HeightMapResolution];
            for (int i = 0; i < chunk.HeightMapResolution; ++i)
                for (int j = 0; j < chunk.HeightMapResolution; ++j)
                {
                    height_map[i, j] = chunk.HeightMap[j * chunk.HeightMapResolution + i]; // problem with terrain coordinates(swaped x and z)
                }
            terrain.terrainData.SetHeights(0, 0, height_map);
        }
    }
}