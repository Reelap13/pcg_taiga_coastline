using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms
{
    public class BiomsAgent : Singleton<BiomsAgent>
    {
        [SerializeField] private Biom[] _bioms;

        public Biom GetBiom(Vector2 position)
        {
            foreach (Biom biom in _bioms)
            {
                if (biom.IsInBiom(position))
                    return biom;
            }

            Debug.LogError($"Position {position} hasn't biom");
            return null;
        }
    }
}