using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.New_Bioms.Creator
{
    public class TextureMapCreator : MonoBehaviour
    {
        public Texture2D Texture;

        public int GetTextureID()
        {
            return TexturesSet.Instance.GetTextureData(Texture).Id;
        }
    }
}