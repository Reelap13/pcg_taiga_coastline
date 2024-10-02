using PCG_Map.Bioms;
using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace PCG_Map.Objects
{
    public class ObjectsAgent : Singleton<ObjectsAgent>
    {
        public List<GameObject> GetObjects(Vector2 start_position, Vector2Int size)
        {
            List<GameObject> objects = new();

            Vector2 position = new();
            for (int x = 0; x < size.x; x++)
            {
                position.x = (int)start_position.x + x;
                for (int y = 0; y < size.y; y++)
                {
                    position.y = (int)start_position.y + y;

                    Biom biom = BiomsAgent.Instance.GetBiom(position);
                    List<ObjectData> field_objects_data = biom.GetObjectsPrefabs(position);
                    List<GameObject> field_objects = CreateObjects(field_objects_data);

                    objects.AddRange(field_objects);
                }
            }

            return objects;
        }

        private List<GameObject> CreateObjects(List<ObjectData> objects_data)
        {
            List<GameObject> objects = new();
            foreach (ObjectData data in objects_data)
            {
                GameObject obj = Instantiate(data.Prefab) as GameObject;
                obj.transform.position = data.Position;
                obj.transform.rotation = data.Rotation;

                objects.Add(obj);
            }

            return objects;
        }
    }
}