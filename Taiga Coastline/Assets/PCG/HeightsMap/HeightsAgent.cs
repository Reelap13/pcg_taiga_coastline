using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Heights
{
    public class HeightsAgent : Singleton<HeightsAgent>
    {
        [SerializeField] private Coastline _coastline;
        [SerializeField] private LandHeights _land_heights;
        [SerializeField] private SeaHeights _sea_heights;

        [field: SerializeField]
        public float TerrainHeight { get; private set; } = 600;
        [field: SerializeField]
        public float SeaHeight { get; private set; } = 50;
        [field: SerializeField]
        public float MinHeight { get; private set; } = 1;

        public Matrix2D GetHeights(Vector2 start_position, Vector2Int size, int heights_map_resolution)
        {
            Matrix2D heights = new(heights_map_resolution);
            Vector2 position = start_position;
            for (int i = 0; i < heights_map_resolution; ++i)
            {
                position.x = start_position.x + size.x * i / (heights_map_resolution - 1);
                for (int j = 0; j < heights_map_resolution; ++j)
                {
                    position.y = start_position.y + size.y * j / (heights_map_resolution - 1);
                    heights[i, j] = GetHeight(position) / TerrainHeight;
                }
            }


            return heights;
        }

        public float GetHeight(Vector2 position)
        {
            if (GetOffsetsFromCoastline(position) >= 0)
                return _land_heights.GetHeight(position);
            else return _sea_heights.GetHeight(position);
        }

        public float GetOffsetsFromCoastline(Vector2 position)
        {
            float coastline_y = _coastline.GetYValue(position.x);
            return position.y - coastline_y;
        }

        public void Initialize()
        {
            _coastline.Initialize();
        }
    }
}