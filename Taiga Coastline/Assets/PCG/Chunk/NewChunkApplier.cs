using PCG_Map.Objects;
using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Collections;
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
            ApplyHeightMap(chunk, terrain);
            ApplyTextureMap(chunk, terrain);
            CreateObjects(chunk, chunk_obj, terrain);

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

            terrain.terrainData.heightmapResolution = chunk.HTMapsResolution;
            terrain.terrainData.size = new Vector3(chunk.Size, Generator.Instance.HeightsAgent.TerrainHeight, chunk.Size);

            return terrain;
        }


        private void ApplyPosition(GameObject chunk_obj, NewChunk chunk)
        {
            chunk_obj.transform.position = new Vector3(chunk.Position.x, 0, chunk.Position.y);
        }

        private void ApplyHeightMap(NewChunk chunk, Terrain terrain)
        {
            float[,] height_map = new float[chunk.HTMapsResolution, chunk.HTMapsResolution];
            for (int i = 0; i < chunk.HTMapsResolution; ++i)
                for (int j = 0; j < chunk.HTMapsResolution; ++j)
                {
                    height_map[i, j] = chunk.HeightMap.GetData(j, i); // problem with terrain coordinates(swaped x and z)
                }
            terrain.terrainData.SetHeights(0, 0, height_map);
        }

        private void ApplyTextureMap(NewChunk chunk, Terrain terrain)
        {
            NativeArray<int> unique_textures = chunk.Textures.ToNativeArray(Allocator.Temp);

            int size = chunk.HTMapsResolution;
            int textures_number = unique_textures.Length;

            terrain.terrainData.alphamapResolution = size;
            TerrainLayer[] terrain_layers = new TerrainLayer[textures_number];
            float[,,] splatmap_data = new float[size, size, textures_number];
            for (int texture_number = 0; texture_number < textures_number; ++texture_number)
            {
                int texture_id = unique_textures[texture_number];

                terrain_layers[texture_number] = new TerrainLayer();
                terrain_layers[texture_number].diffuseTexture = TexturesSet.Instance.GetTextureData(texture_id).Texture;
                terrain_layers[texture_number].tileSize = new(2, 2); //can be changed

                for (int x = 0; x < size; ++x)
                {
                    for (int y = 0; y < size; ++y)
                    {
                        int xy_texture_id = chunk.TextureMap.GetData(y, x); // problem with terrain coordinates(swaped x and z)
                        
                        splatmap_data[x, y, texture_number] = xy_texture_id == texture_id ? 1 : 0;
                    }
                }
            }
            unique_textures.Dispose();

            terrain.terrainData.terrainLayers = terrain_layers;
            terrain.terrainData.SetAlphamaps(0, 0, splatmap_data);
        }

        private void CreateObjects(NewChunk chunk, GameObject chunk_obj, Terrain terrain)
        {
            var random = new Unity.Mathematics.Random((uint)Generator.Instance.Seed + (uint)(chunk.Position.x * chunk.Size) + (uint)(chunk.Position.y * 2));

            foreach (ObjectData obj_data in chunk.Objects)
            {
                GameObject[] obj_prefabs = ObjectsSet.Instance.GetPrefabs(obj_data.PrefabID);
                GameObject obj = Instantiate(obj_prefabs[random.NextInt(0, obj_prefabs.Length)]) as GameObject;

                obj.transform.SetParent(chunk_obj.transform, false);

                Vector3 position = new Vector3(obj_data.Position.x, obj_data.Height, obj_data.Position.y);
                if (position.y == -1)
                    position.y = terrain.SampleHeight(position);
                obj.transform.position = position;

                Vector3 rotation = obj.transform.eulerAngles;
                rotation.y = obj_data.Rotation;
                obj.transform.rotation = Quaternion.Euler(rotation);
            }
            Debug.Log($"Hi, {chunk.Objects.Length} objects was generated!");
        }
    }
}