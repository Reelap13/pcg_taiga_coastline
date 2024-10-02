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
        [SerializeField] private PlayerTracker _player_tracker;

        [SerializeField] private int radius = 3;
        [SerializeField] private int _radius_number_to_disactivate = 3;

        private Dictionary<Vector2Int, Chunk> _created_chunks;
        private Dictionary<Vector2Int, Chunk> _active_chunks;
        private List<Vector2Int> _creating_chunks;

        private Vector2Int _target_chunk_position;

        public void Initialize()
        {
            _created_chunks = new();
            _active_chunks = new();
            _creating_chunks = new();
            _player_tracker.OnMovingToOtherChunk.AddListener(UpdateTargetChunk);
            _player_tracker.Initialize();
        }

        private void UpdateTargetChunk(Vector2Int position)
        {
            _target_chunk_position = position;
            StartCoroutine(LoadChunks(position));
        }

        private IEnumerator LoadChunks(Vector2Int start_position)
        {
            DisactivateAllChunks();

            List<Vector2Int> created_chunks = new();
            List<Vector2Int> noncreated_chunks = new();
            Vector2Int position = Vector2Int.zero;
            for (int i = -radius; i <= radius; ++i)
            {
                position.x = start_position.x + i * ChunkSize;
                for (int j = -radius; j <= radius; ++j)
                {
                    position.y = start_position.y + j * ChunkSize;
                    if (_created_chunks.ContainsKey(position))
                        created_chunks.Add(position);
                    else if (_creating_chunks.Contains(position))
                        continue;
                    else
                    {
                        _creating_chunks.Add(position);
                        noncreated_chunks.Add(position);
                    }
                }
            }

            foreach (var chunk in created_chunks)
                _created_chunks[chunk].ChunkObj.SetActive(true);

            foreach (var chunk in noncreated_chunks)
            {
                StartCoroutine(CreateChunk(chunk));
                yield return null;
            }
        }

        private IEnumerator CreateChunk(Vector2Int position)
        {
            Chunk new_chunk = _chunks_factory.CreateChunk(position);
            yield return null;
            _created_chunks.Add(position, new_chunk);
            if (IsInTargetsChunks(position))
                _active_chunks.Add(position, new_chunk);
            else new_chunk.ChunkObj.SetActive(false);
            //_active_chunks.Add(position, new_chunk);
            _creating_chunks.Remove(position);
        }

        private void DisactivateAllChunks()
        {
            foreach(Vector2Int position in _created_chunks.Keys)
                if (!IsInTargetsChunks(position))
                    _created_chunks[position].ChunkObj.SetActive(false);

            _active_chunks.Clear();
        }

        private bool IsInTargetsChunks(Vector2Int position)
        {
            return (Mathf.Abs(_target_chunk_position.x - position.x) + Mathf.Abs(_target_chunk_position.y - position.y)) / ChunkSize <= radius * _radius_number_to_disactivate;
        }
    }
}