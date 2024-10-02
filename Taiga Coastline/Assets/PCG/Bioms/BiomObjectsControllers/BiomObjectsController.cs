using PCG_Map.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms.Objects
{
    public abstract class BiomObjectsController : MonoBehaviour
    {
        public abstract List<ObjectData> GetObjectsPrefabs(Vector2 position);

        protected Vector3 GetRandomPosition(Vector2 position)
        {
            float x = Random.value - 0.5f;
            float z = Random.value - 0.5f;
            
            return new Vector3(position.x + x, 0, position.y + z);
        }

        protected Quaternion GetRandomRotation()
        {
            return Quaternion.Euler(0, Random.Range(0, 360), 0);
        }
    }
}