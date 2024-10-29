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
        public NativeHashMap<int, int> BiomsTextures;

        public void AddBiomTemplateTexture(int biom_id, int texture_id)
        {
            BiomsTextures.Add(biom_id, texture_id);
        }

        public int GetTexture(int biom_id)
        {
            return BiomsTextures[biom_id];
        }

        public void Dispose()
        {
            BiomsTextures.Dispose();
        }
    }
}