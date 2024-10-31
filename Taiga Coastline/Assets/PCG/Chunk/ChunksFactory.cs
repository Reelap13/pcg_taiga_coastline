using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace PCG_Map.Chunk
{
    public class ChunksFactory : MonoBehaviour
    {
        [NonSerialized] public UnityEvent<ChunkData> OnChunkCreating = new UnityEvent<ChunkData>();

        [field: SerializeField] 
        public ChunksManager ChunksManager { get; private set; }

        [SerializeField] private ChunkGenerator _chunks_generator;
        [SerializeField] private ChunksLoader _chunks_loader;
        [SerializeField] private NewChunkApplier _chunks_applier;
        [SerializeField] private int _height_map_coefficient = 2;
        [SerializeField] private int _chunk_allying_per_frame = 4;

        public int Size => ChunksManager.ChunkSize;
        public int HeightMapResolution => ChunksManager.ChunkSize * _height_map_coefficient + 1;

        private List<NewChunk> _generating_chunks = new();

        private void Update()
        {
            CheckChunksToFinishGeneration();
        }

        private void CheckChunksToFinishGeneration()
        {
            int k = 0;
            List<NewChunk> finished_generation_chunks = new();
            foreach(NewChunk chunk in _generating_chunks)
            {
                bool is_finish_generation = true;
                foreach (var job_handler in chunk.UnfinishedJobs)
                {
                    if (!job_handler.IsCompleted)
                    {
                        is_finish_generation = false;
                        break;
                    }
                }

                if (!is_finish_generation)
                    continue;

                ++k;
                if (k > _chunk_allying_per_frame)
                    break;
                finished_generation_chunks.Add(chunk);
            }

            foreach (NewChunk chunk in finished_generation_chunks)
            {
                foreach (var job_handler in chunk.UnfinishedJobs)
                    job_handler.Complete();
                _generating_chunks.Remove(chunk);
                ChunkData data = _chunks_applier.ApplyToChunk(chunk);
                OnChunkCreating.Invoke(data);
            }
        }

        public void CreateChunk(Vector2 position)
        {
            NewChunk chunk = GenerateChunk(position);
            _generating_chunks.Add(chunk);
        }

        private NewChunk GenerateChunk(Vector2 position) => _chunks_generator.GenerateChunk(position);
        private NewChunk LoadChunk(Vector2 position) => _chunks_loader.LoadChunk(position);
        private bool CheckToLoaded(Vector2 position) => _chunks_loader.CheckToLoaded(position);
    }
}