using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PCG_Map.Chunk
{
    public class ChunksManager : MonoBehaviour
    {
        [field: SerializeField]
        public int ChunkSize { get; private set; } = 32;

        [SerializeField] private ChunksFactory _chunks_factory;

        private void Start()
        {
            _chunks_factory.CreateChunk(new(0, 0));
        }
    }
}