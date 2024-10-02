using PCG_Map.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PCG_Map.Heights;

namespace PCG_Map.Bioms.Objects
{
    public class WaterSpawner : BiomObjectsController
    {
        [SerializeField] private GameObject _water_prefab;
        public override List<ObjectData> GetObjectsPrefabs(Vector2 position)
        {
            Vector3 pos = new Vector3(position.x + 0.5f, HeightsAgent.Instance.SeaHeight, position.y + 0.5f);
            return new List<ObjectData>() { new ObjectData(_water_prefab, pos) }; 
        }
    }
}