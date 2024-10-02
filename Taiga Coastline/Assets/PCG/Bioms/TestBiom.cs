using PCG_Map.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms
{
    public class TestBiom : Biom
    {
        [SerializeField] private Texture2D _test_texture;
        [SerializeField] private GameObject _test_biom_object;

        public override List<ObjectData> GetObjectsPrefabs(Vector2 position)
        {
            List<ObjectData> objects = new();
            if (Random.value > 0.95f)
                objects.Add(new (_test_biom_object, 
                    new Vector3(position.x, 0, position.y), 
                    Quaternion.identity));

            return objects;
        }

        public override Texture2D GetTexture(Vector2 position)
        {
            return _test_texture;
        }

        public override bool IsInBiom(Vector2 position)
        {
            return true;
        }
    }
}