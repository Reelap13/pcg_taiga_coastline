using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Objects
{
    public class ObjectsSet : Singleton<ObjectsSet>
    {
        private Dictionary<int, GameObject[]> _id_to_prefabs;

        private int id;

        public void Initialize()
        {
            _id_to_prefabs = new Dictionary<int, GameObject[]>();
            id = 0;
        }

        public int RegisterPrefabs(GameObject[] prefabs)
        {
            _id_to_prefabs.Add(++id, prefabs);
            return id;
        }

        public GameObject[] GetPrefabs(int id)
        {
            return _id_to_prefabs[id];
        }
    }
}