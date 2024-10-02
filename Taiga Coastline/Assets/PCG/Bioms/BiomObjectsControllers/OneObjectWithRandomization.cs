using PCG_Map.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms.Objects
{
    public class OneObjectWithRandomization : BiomObjectsController
    {
        [SerializeField] private GameObject _test_biom_object;
        [SerializeField] private float _probability = 0.05f;

        public override List<ObjectData> GetObjectsPrefabs(Vector2 position)
        {
            List<ObjectData> objects = new();

            Random.InitState((int)(Generator.Instance.Seed + position.x * 122 + position.y * 145 + Mathf.Pow(position.x + 1, 2) + Mathf.Pow(position.y + 1, 3)));
            if (Random.value > 1 - _probability)
                objects.Add(new(_test_biom_object,
                    new Vector3(position.x, 0, position.y),
                    Quaternion.identity));

            return objects;
        }
    }
}