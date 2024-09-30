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

        [SerializeField] private int radius = 3;

        public void CreateChunks()
        {
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    _chunks_factory.CreateChunk(ChunkSize * new Vector2(i, j));
                }
            }
        }
    }
}