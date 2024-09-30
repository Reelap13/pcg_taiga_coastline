using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG_Map.Chunk
{
    public class ChunksFactory : MonoBehaviour
    {
        [field: SerializeField] 
        public ChunksManager ChunksManager { get; private set; }

        [SerializeField] private ChunkGenerator _chunks_generator;
        [SerializeField] private ChunksLoader _chunks_loader;

        public Vector2Int Size => new(ChunksManager.ChunkSize, ChunksManager.ChunkSize);
        public int HeightsMapResolution => ChunksManager.ChunkSize * 1  + 1;

        public Chunk CreateChunk(Vector2 position)
        {
            Chunk chunk;
            if (CheckToLoaded(position))
                chunk = LoadChunk(position);
            else chunk = GenerateChunk(position);

            ApplyChunk(chunk);

            return chunk;
        }

        private void ApplyChunk(Chunk chunk)
        {
            ChunkApplier applier = new(chunk);
            applier.Apply();
        }

        private Chunk GenerateChunk(Vector2 position) => _chunks_generator.GenerateChunk(position);
        private Chunk LoadChunk(Vector2 position) => _chunks_loader.LoadChunk(position);
        private bool CheckToLoaded(Vector2 position) => _chunks_loader.CheckToLoaded(position);
    }
}