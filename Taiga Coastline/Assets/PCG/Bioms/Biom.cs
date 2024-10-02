using PCG_Map.Bioms.Area;
using PCG_Map.Bioms.Objects;
using PCG_Map.Bioms.Textures;
using PCG_Map.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms
{
    public class Biom : MonoBehaviour
    {
        [SerializeField] public BiomType _type;
        [SerializeField] private BiomTexturesController _textures_controller;
        [SerializeField] private BiomAreaController _area_controller;
        [SerializeField] private BiomObjectsController _objects_controller;

        public bool IsInBiom(Vector2 position) => _area_controller.IsInBiom(position);

        public Texture2D GetTexture(Vector2 position) => _textures_controller.GetTexture(position);
        

        public List<ObjectData> GetObjectsPrefabs(Vector2 position)
        {
            return _objects_controller.GetObjectsPrefabs(position);
        }

        public BiomType Type { get { return _type; } }
    }
}