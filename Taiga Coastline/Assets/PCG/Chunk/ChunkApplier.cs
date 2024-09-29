using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Chunk
{
    public class ChunkApplier
    {
        private Chunk _chunk;

        public GameObject ChunkObj => _chunk.ChunkObj;
        public Terrain Terrain => _chunk.Terrain;
        public Matrix2D Heights => _chunk.Heights;
        public Vector2 Position => _chunk.Position;
        public Vector2Int Size => _chunk.Size;
        public int HeightsMapReoslution => _chunk.HeightsMapResolution;
        public List<GameObject> Objects => _chunk.Objects;

        public ChunkApplier(Chunk chunk)
        {
            _chunk = chunk;
        }

        public void Apply()
        {
            SetChilds();
            Debug.Log("Set childs");
            ApplySize();
            ApplyPosition();
            Debug.Log("Set position");
            ApplyHeights();
            Debug.Log("Set heights map");
        }

        private void SetChilds()
        {
            Terrain.transform.SetParent(ChunkObj.transform);
            foreach (var obj in Objects)
                obj.transform.SetParent(ChunkObj.transform);
        }

        private void ApplySize()
        {
            Terrain.terrainData.size = new Vector3(Size.x, Terrain.terrainData.size.y, Size.y);
        }

        private void ApplyPosition()
        {
            Terrain.transform.position = new Vector3(Position.x, 0, Position.y);
        }

        private void ApplyHeights()
        {
            Terrain.terrainData.heightmapResolution = HeightsMapReoslution;

            float[,] height_map = new float[HeightsMapReoslution, HeightsMapReoslution];
            for (int i = 0; i < HeightsMapReoslution; ++i)
                for (int j = 0; j < HeightsMapReoslution; ++j)
                    height_map[i, j] = Heights[i, j];
            Terrain.terrainData.SetHeights(0, 0, height_map);
        }

        private void ApplyTextures()
        {

        }

        private void ApplyObjects()
        {

        }
    }
}