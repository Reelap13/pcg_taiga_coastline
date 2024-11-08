using PCG_Map.Algorithms;
using PCG_Map.Objects;
using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace PCG_Map.New_Bioms
{
    public struct BiomTemplate
    {
        public int ID;
        public BiomType Type;

        public HeightMapAlgorithmType HeightsMapAlgorithmType;
        public TextureData Texture;

        public NativeArray<ObjectPrefabData> Objects;

        public void Dispose()
        {
            Objects.Dispose();
        }
    }
}