using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms.Textures
{
    public abstract class BiomTexturesController : MonoBehaviour
    {   
        public abstract Texture2D GetTexture(Vector2 position);
    }
}