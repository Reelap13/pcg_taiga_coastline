using PCG_Map.Objects;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace PCG_Map.New_Bioms.Creator
{
    public class ObjectsCreator : MonoBehaviour
    {
        private ObjectPrefabSetter[] _prefabs;

        public NativeArray<ObjectPrefabData> GetObjectsData()
        {
            if (_prefabs == null)
                Load();

            NativeArray<ObjectPrefabData> objects_data = new NativeArray<ObjectPrefabData>(_prefabs.Length, Allocator.Persistent);
            for (int i = 0; i < _prefabs.Length; ++i)
            {
                var data = _prefabs[i].Data;
                var prefabs = _prefabs[i].Prefabs;
                objects_data[i] = new ObjectPrefabData
                {
                    PrefabID = ObjectsSet.Instance.RegisterPrefabs(prefabs),
                    SpawnAlgorithmType = data.SpawnAlgorithmType,
                    PlacementAlgorithmType = data.PlacementAlgorithmType,
                    HeightAlgorithmType = data.HeightAlgorithmType,
                    SpawnFrequency = data.SpawnFrequency,
                    Height = data.Height,
                    HeightOffset = data.HeightOffset,
                    TerrainHeightBorders = data.TerrainHeightBorders,
                    ScaleInterval = data.ScaleInterval,
                };
            }

            return objects_data;
        }

        private void Load()
        {
            _prefabs = GetComponentsInChildren<ObjectPrefabSetter>();
        }
    }
}