using PCG_Map.Chunk;
using PCG_Map.Heights;
using PCG_Map.New_Bioms;
using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace PCG_Map
{
    public class Generator : Singleton<Generator>
    {
        [SerializeField] private ChunksManager _chunk_manager;
        [field: SerializeField] public HeightsAgent HeightsAgent;
        [field: SerializeField] public TexturesAgent TexturesAgent;

        [field: SerializeField] public BiomsController Bioms;
        [field: SerializeField] public int Seed;

        private void Start()
        {
            HeightsAgent.Initialize();
            Bioms.Initialize();
            _chunk_manager.Initialize();
            //_chunk_manager.Initialize();
            //StartCoroutine(GenerateMainAgent());
        }

        private IEnumerator GenerateMainAgent()
        {
            
            HeightsAgent.Initialize();
            TexturesAgent.Initialize();
            yield return null;
            _chunk_manager.Initialize();
        }
    }
}