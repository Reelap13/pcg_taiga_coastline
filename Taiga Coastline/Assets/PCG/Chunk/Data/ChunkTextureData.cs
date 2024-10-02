using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Chunk
{
    public class ChunkTextureData
    {
        private TextureData _data;
        private Matrix2D _matrix;

        public ChunkTextureData(TextureData data, Matrix2D matrix)
        {
            _data = data;
            _matrix = matrix;
        }

        public TextureData Data { get { return _data; } }
        public Texture2D Texture { get { return _data.Texture; } }
        public int Id { get { return _data.Id; } }
        public Matrix2D Matrix { get { return _matrix; } }
    }
}