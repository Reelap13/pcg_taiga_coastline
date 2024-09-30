using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Heights
{
    public abstract class HeightsProcessor : MonoBehaviour
    {
        [field: SerializeField]
        public HeightsAgent HeightsAgent { get; private set; }

        public float TerrainHeight => HeightsAgent.TerrainHeight;
        public float SeaHeight => HeightsAgent.SeaHeight;
        public float MinHeight => HeightsAgent.MinHeight;
        public float GetOffsetsFromCoastline(Vector2 position) => HeightsAgent.GetOffsetsFromCoastline(position);

        public float GetValue(Vector2 position)
        {
            float height = CheckBoarder(GetHeight(position));
            return ParseHeightToValue(height);
        }

        public abstract float GetHeight(Vector2 position);

        public float ParseValueToHeight(float value) // parse value from 0 to 1 to terrain heights
        {
            return value * TerrainHeight;
        }

        public float ParseHeightToValue(float height) // parse terrain height to value from 0 to 1
        {
            return height / TerrainHeight;
        }

        public float CheckBoarder(float value)
        {
            value = Mathf.Min(value, TerrainHeight);
            value = Mathf.Max(value, TerrainHeight);

            return value;
        }
    }
}