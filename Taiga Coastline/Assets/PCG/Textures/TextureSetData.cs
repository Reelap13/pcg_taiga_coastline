using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Textures
{
    public class TextureSetData
    {
        private Texture2D _texture;
        private int _id;

        public TextureSetData(Texture2D texture, int id)
        {
            _texture = texture;
            _id = id;
        }

        public Texture2D Texture { get { return _texture; } }
        public int Id { get { return _id; } }
    }
}