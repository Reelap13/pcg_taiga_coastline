using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Objects
{
    [BurstCompile]
    public struct ObjectsAlgorithmData
    {
        private NativeArray<ObjectPrefabData> _prefabs;
        private NativeHashMap<int, int2> _links_to_prefab;

        private int _id;

        public ObjectsAlgorithmData(NativeArray<ObjectPrefabData> prefabs, NativeHashMap<int, int2> links_to_prefab)
        {
            _prefabs = prefabs;
            _links_to_prefab = links_to_prefab;
            _id = 0;
        }

        public void RegisterBiomTemplate(int biom_template_id, NativeArray<ObjectPrefabData> prefabs)
        {
            _links_to_prefab.Add(biom_template_id, new int2(_id, _id + prefabs.Length));

            for (int i = 0; i < prefabs.Length; ++i)
                _prefabs[_id + i] = prefabs[i];

            _id += prefabs.Length;
        }

        public NativeArray<ObjectPrefabData> GetBiomObjects(int biom_template_id)
        {
            int2 indexes = _links_to_prefab[biom_template_id];
            int length = indexes.y - indexes.x;
            NativeArray<ObjectPrefabData> objects = new NativeArray<ObjectPrefabData>(length, Allocator.Temp);
            for (int i = 0; i < length; ++i)
                objects[i] = _prefabs[indexes.x + i];

            return objects;
        }

        public void Dispose()
        {
            _prefabs.Dispose();
            _links_to_prefab.Dispose();
        }
    }
}