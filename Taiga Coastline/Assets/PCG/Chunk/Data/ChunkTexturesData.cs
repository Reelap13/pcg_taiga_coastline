using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PCG_Map.Chunk
{
    public class ChunkTexturesData
    {
        private Dictionary<int, ChunkTextureData> _textures_data;
        private Vector2Int _size;

        public ChunkTexturesData(Vector2Int size)
        {
            _size = size;

            _textures_data = new();
        }

        public void AddTexture(float value, Vector2Int position, TextureData texture_data)
        {
            if (!_textures_data.ContainsKey(texture_data.Id))
                AddTextureData(texture_data);

            ChunkTextureData data = _textures_data[texture_data.Id];
            data.Matrix[position.x, position.y] = Mathf.Clamp01(value);
        }

        private void AddTextureData(TextureData texture_data)
        {
            _textures_data[texture_data.Id] = new ChunkTextureData(texture_data, new(_size));
        }

        public ChunkTextureData[] GetChunkTexturesData()
        {
            return _textures_data.Values.ToArray();
        }
    }
}