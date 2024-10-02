using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PCG_Map.Chunk
{
    public class PlayerTracker : MonoBehaviour
    {
        public UnityEvent<Vector2Int> OnMovingToOtherChunk = new();

        [field: SerializeField]
        public ChunksManager ChunkManager { get; private set; }

        [SerializeField] private Transform _target;

        private Vector2Int _target_chunk_position;
        private bool _is_initialized = false;

        public void Initialize()
        {
            _is_initialized = true;
            _target_chunk_position = GetTargetChunkPosition();
            OnMovingToOtherChunk.Invoke(_target_chunk_position);
        }

        private void Update()
        {
            if (!_is_initialized) return;

            Vector2Int new_position = GetTargetChunkPosition();

            if (new_position != _target_chunk_position)
            {
                _target_chunk_position = new_position;
                OnMovingToOtherChunk.Invoke(new_position);
            }
        }

        private Vector2Int GetTargetChunkPosition()
        {
            Vector2Int current_position = new Vector2Int((int)_target.transform.position.x, (int)_target.transform.position.z);
            return current_position - new Vector2Int(current_position.x % ChunkManager.ChunkSize, current_position.y % ChunkManager.ChunkSize);
        }
    }
}