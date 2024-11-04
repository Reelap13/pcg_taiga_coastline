using PCG_Map.Objects;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PCG_Map.New_Bioms.Creator
{
    public class ObjectPrefabSetter : MonoBehaviour
    {
        [SerializeField] private GameObject[] _prefabs;
        [SerializeField] private ObjectPrefabSetterData _data;

        public ObjectPrefabSetterData Data { get { return _data; } }
        public GameObject[] Prefabs { get { return _prefabs; } }

#if UNITY_EDITOR
        [CustomEditor(typeof(ObjectPrefabSetter))]
        public class ObjectPrefabSetterEditor : Editor
        {
            private ObjectPrefabSetterData _default_data = new();
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                var creator = (ObjectPrefabSetter)target;
                var data = creator._data;

                EditorGUILayout.LabelField("Object Settings", EditorStyles.boldLabel);

                data.SpawnAlgorithmType = (ObjectSpawnAlgorithmType)EditorGUILayout.EnumPopup("Spawn Algorithm", data.SpawnAlgorithmType);
                switch (data.SpawnAlgorithmType)
                {
                    case ObjectSpawnAlgorithmType.BY_SPAWN_FREQUENCY:
                        data.SpawnFrequency = EditorGUILayout.FloatField("Spawn Frequency", data.SpawnFrequency);
                        break;
                    default:
                        data.SpawnFrequency = _default_data.SpawnFrequency;
                        break;
                }

                data.PlacementAlgorithmType = (ObjectPlacementAlgorithmType)EditorGUILayout.EnumPopup("Placement Algorithm", data.PlacementAlgorithmType);

                data.HeightAlgorithmType = (ObjectHeightAlgorithmType)EditorGUILayout.EnumPopup("Height Algorithm", data.HeightAlgorithmType);
                switch (data.HeightAlgorithmType)
                {
                    case ObjectHeightAlgorithmType.PRESETTED:
                        data.Height = EditorGUILayout.FloatField("Height", data.Height);
                        break;
                    case ObjectHeightAlgorithmType.ZERO:
                        data.Height = 0;
                        break;
                    default:
                        data.Height = _default_data.Height;
                        break;
                }
                data.HeightOffset = EditorGUILayout.FloatField("Height Offset", data.HeightOffset);

                if (GUI.changed)
                    EditorUtility.SetDirty(creator);
            }
        }
#endif
    }
    [System.Serializable]
    public class ObjectPrefabSetterData
    {
        public ObjectSpawnAlgorithmType SpawnAlgorithmType = ObjectSpawnAlgorithmType.BY_SPAWN_FREQUENCY;
        public ObjectPlacementAlgorithmType PlacementAlgorithmType = ObjectPlacementAlgorithmType.RANDOM_POSITION_IN_STEP_AREA;
        public ObjectHeightAlgorithmType HeightAlgorithmType = ObjectHeightAlgorithmType.TERRAIN;
        public float SpawnFrequency = 1f;
        public float Height = -1f;
        public float HeightOffset = 0f;
        public Vector2 TerrainHeightBorders = new(0, 600);
    }
}