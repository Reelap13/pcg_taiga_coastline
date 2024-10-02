using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms.Textures
{
    public class OneTexture : BiomTexturesController
    {
        [SerializeField] private Texture2D _texture;

        public override Texture2D GetTexture(Vector2 position)
        {
            return _texture;
        }
    }
}