using PCG_Map.Chunk;
using PCG_Map.Heights;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map
{
    public class Generator : Singleton<Generator>
    {
        [SerializeField] private ChunksManager _chunk_manager;
        [field: SerializeField] public HeightsAgent HeightsAgent;

        [field: SerializeField] public int Seed;

        private void Start()
        {
            StartCoroutine(GenerateMainAgent());
        }

        private IEnumerator GenerateMainAgent()
        {
            HeightsAgent.Initialize();
            yield return null;
            _chunk_manager.CreateChunks();

            Debug.Log(HeightsAgent.GetHeight(new(0, 0)));
            Debug.Log(HeightsAgent.GetHeight(new(55, 55)));
            Debug.Log(HeightsAgent.GetHeight(new(-55, -55)));
        }
    }
}