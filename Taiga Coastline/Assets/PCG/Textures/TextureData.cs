using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

namespace PCG_Map.Textures
{
    [BurstCompile]
    public struct TextureData
    {
        public TextureAlgorithmType Type;
        public int Length;
        public int FirstTexture;
        public float FirstMaxHeight;
        public int SecondTexture;
        public float SecondMaxHeight;
        public int ThirdTexture;

        public TextureData(TextureCreatorData data)
        {
            Type = data.Type;
            Length = data.Length;
            FirstTexture = TexturesSet.Instance.GetTextureData(data.FirstTexture).Id;
            FirstMaxHeight = 0;
            SecondTexture = 0;
            SecondMaxHeight = 0;
            ThirdTexture = 0;

            if (Length > 1)
            {
                FirstMaxHeight = data.FirstMaxHeight;
                SecondTexture = TexturesSet.Instance.GetTextureData(data.SecondTexture).Id;
                SecondMaxHeight = data.SecondMaxHeight;
            }
            if (Length > 2)
            {
                ThirdTexture = TexturesSet.Instance.GetTextureData(data.ThirdTexture).Id;
            }
        }
    }

    [System.Serializable]
    public class TextureCreatorData
    {

        public TextureAlgorithmType Type;
        public int Length;
        public Texture2D FirstTexture;
        public float FirstMaxHeight;
        public Texture2D SecondTexture;
        public float SecondMaxHeight;
        public Texture2D ThirdTexture;
    }
}