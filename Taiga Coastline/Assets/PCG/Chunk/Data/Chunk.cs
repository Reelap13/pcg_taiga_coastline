using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Chunk
{
    public class Chunk
    {
        private readonly GameObject _chunk_obj;
        private readonly Vector2 _position;
        private readonly Vector2Int _size;
        private readonly int _heights_map_resolution;
        private readonly Matrix2D _heights;
        private readonly ChunkTexturesData _textures;
        private readonly Terrain _terrain;
        private readonly List<GameObject> _objects;


        public Chunk(GameObject chunk_obj, Vector2 position, Vector2Int size, int heights_map_resolution, Matrix2D heights, ChunkTexturesData textures, Terrain terrain, List<GameObject> objects)
        {
            _chunk_obj = chunk_obj;
            _position = position;
            _size = size;
            _heights_map_resolution = heights_map_resolution;
            _heights = heights;
            _textures = textures;
            _terrain = terrain;
            _objects = objects;
        }

        public GameObject ChunkObj { get { return _chunk_obj; } }  
        public Vector2 Position { get { return _position; } }
        public Vector2Int Size { get { return _size; } }
        public int HeightsMapResolution { get { return _heights_map_resolution; } }
        public Matrix2D Heights { get { return _heights; } }
        public ChunkTexturesData TexturesData { get { return _textures; } }
        public Terrain Terrain { get { return _terrain; } }
        public List<GameObject> Objects { get { return _objects; } }
    }
}