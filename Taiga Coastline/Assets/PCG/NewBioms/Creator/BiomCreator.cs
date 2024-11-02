using PCG_Map.Algorithms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.New_Bioms.Creator
{

    public class BiomCreator : MonoBehaviour
    {
        [SerializeField] private BiomType _biom_type;
        [SerializeField] private HeightMapCreator _height_map;
        [SerializeField] private TextureMapCreator _texture_map;
        [SerializeField] private ObjectsCreator _objects;

        public BiomTemplate GetBiomTemplate()
        {
            return new BiomTemplate()
            {
                Type = _biom_type,
                HeightsMapAlgorithmType = _height_map.Type,
                TextureID = _texture_map.GetTextureID(),
                Objects = _objects.GetObjectsData()
            };
        }

        public IHeightMapAlgorithm GetHeightsMapAlgorithm() { return _height_map.GetHeightsMapAlgorithm(); }
    }
}