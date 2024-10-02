using PCG_Map.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms.Objects
{
    public abstract class BiomObjectsController : MonoBehaviour
    {
        public abstract List<ObjectData> GetObjectsPrefabs(Vector2 position);
    }
}