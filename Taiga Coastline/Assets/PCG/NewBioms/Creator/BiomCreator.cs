using PCG_Map.Algorithms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.New_Bioms.Creator
{

    public class BiomCreator : MonoBehaviour
    {
        [SerializeField] private BiomType _biom_type;
        [SerializeField] private HeightsMapCreator _heights_map;

        public BiomTemplate GetBiomTemplate()
        {
            return new BiomTemplate()
            {
                Type = _biom_type,
                HeightsMapAlgorithmType = _heights_map.Type
            };
        }

        public IHeightsMapAlgorithm GetHeightsMapAlgorithm() { return _heights_map.GetHeightsMapAlgorithm(); }
    }
}