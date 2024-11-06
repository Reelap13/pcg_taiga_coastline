using PCG_Map.Algorithms;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace PCG_Map.Textures
{
    [BurstCompile]
    public struct TextureMapAlgorithmData
    {
        public NativeHashMap<int, TextureData> BiomsTextures;

        public void AddBiomTemplateTexture(int biom_id, TextureData texture)
        {
            BiomsTextures.Add(biom_id, texture);
        }

        public TextureData GetTexture(int biom_id)
        {
            return BiomsTextures[biom_id];
        }

        public void Dispose()
        {
            BiomsTextures.Dispose();
        }
    }
}