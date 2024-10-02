using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms.Objects
{
    public class BiomObjectData : MonoBehaviour
    {
        [SerializeField] private GameObject[] _prefab;
        [SerializeField] private float _frequency = 0.5f;   
        
        public GameObject Prefab { get { return _prefab[Random.Range(0, _prefab.Length)]; } }
        public float Frequency { get { return _frequency; } }
    }
}