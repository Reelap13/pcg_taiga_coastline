using PCG_Map.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms
{
    public abstract class Biom : MonoBehaviour
    {
        [SerializeField] public BiomType _type;

        public abstract bool IsInBiom(Vector2 position);

        public abstract Texture2D GetTexture(Vector2 position);
        public abstract List<ObjectData> GetObjectsPrefabs(Vector2 position);

        public BiomType Type { get { return _type; } }
    }
}