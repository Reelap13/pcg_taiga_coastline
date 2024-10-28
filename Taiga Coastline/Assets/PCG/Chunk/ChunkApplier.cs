using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace PCG_Map.Chunk
{
    public class ChunkApplier
    {
        private Chunk _chunk;

        public GameObject ChunkObj => _chunk.ChunkObj;
        public Terrain Terrain => _chunk.Terrain;
        public NativeArray<float> Heights => _chunk.Heights;
        public ChunkTexturesData TexturesData => _chunk.TexturesData;
        public Vector2 Position => _chunk.Position;
        public Vector2Int Size => _chunk.Size;
        public int HeightsMapReoslution => _chunk.HeightsMapResolution;
        public List<GameObject> Objects => _chunk.Objects;

        public ChunkApplier(Chunk chunk)
        {
            _chunk = chunk;
        }

        public void Apply()
        {
            SetChilds();
            ApplySize();
            ApplyPosition();
            ApplyHeights();
            //ApplyTextures();
            //ApplyObjects();
        }

        private void SetChilds()
        {
            Terrain.transform.SetParent(ChunkObj.transform);
        }

        private void ApplySize()
        {
            Terrain.terrainData.heightmapResolution = HeightsMapReoslution;
            Terrain.terrainData.size = new Vector3(Size.x, Generator.Instance.HeightsAgent.TerrainHeight, Size.y);
        }

        private void ApplyPosition()
        {
            ChunkObj.transform.position = new Vector3(Position.x, 0, Position.y);
        }

        private void ApplyHeights()
        {
            float[,] height_map = new float[HeightsMapReoslution, HeightsMapReoslution];
            for (int i = 0; i < HeightsMapReoslution; ++i)
                for (int j = 0; j < HeightsMapReoslution; ++j)
                {
                    height_map[i, j] = Heights[j * HeightsMapReoslution + i]; // problem with terrain coordinates(swaped x and z)
                }
            Terrain.terrainData.SetHeights(0, 0, height_map);

            Heights.Dispose();
        }

        private void ApplyTextures()
        {
            ChunkTextureData[] textures = TexturesData.GetChunkTexturesData();
            TerrainLayer[] terrain_layers = new TerrainLayer[textures.Length];

            Terrain.terrainData.alphamapResolution = Size.x;
            float[,,] splatmap_data = new float[Size.x, Size.y, textures.Length];
            for (int texture_number = 0; texture_number < textures.Length; ++texture_number)
            {
                terrain_layers[texture_number] = new TerrainLayer();
                terrain_layers[texture_number].diffuseTexture = textures[texture_number].Texture;
                terrain_layers[texture_number].tileSize = new(2, 2); //can be changed

                Matrix2D texture_alpha_matrix = textures[texture_number].Matrix;
                for (int x = 0; x < Size.x; ++x)
                {
                    for (int y = 0; y < Size.y; ++y)
                    {
                        float alpha_value = texture_alpha_matrix[y, x]; // problem with terrain coordinates(swaped x and z)
                        alpha_value = Mathf.Clamp01(alpha_value);

                        splatmap_data[x, y, texture_number] = alpha_value;
                    }
                }
            }

            Terrain.terrainData.terrainLayers = terrain_layers;
            Terrain.terrainData.SetAlphamaps(0, 0, splatmap_data);
        }

        private void ApplyObjects()
        {
            foreach (GameObject obj in Objects)
            {
                Vector3 position = obj.transform.position;
                if (obj.transform.position.y == 0)
                    position.y = Terrain.SampleHeight(position);

                obj.transform.position = position;
                obj.transform.SetParent(ChunkObj.transform);
            }
        }
    }
}