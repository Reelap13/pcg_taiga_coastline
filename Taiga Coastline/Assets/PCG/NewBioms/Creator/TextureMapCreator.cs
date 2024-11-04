using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCG_Map.Objects;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PCG_Map.New_Bioms.Creator
{
    public class TextureMapCreator : MonoBehaviour
    {
        public TextureCreatorData TextureData;

        public TextureData GetTexture()
        {
            return new(TextureData);
        }


#if UNITY_EDITOR
        [CustomEditor(typeof(ObjectPrefabSetter))]
        public class TextureMapCreatorEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
            }
        }
#endif
    }
}