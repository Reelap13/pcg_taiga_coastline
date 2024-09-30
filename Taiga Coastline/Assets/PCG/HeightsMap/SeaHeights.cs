using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Heights
{
    public class SeaHeights : HeightsProcessor
    {
        [SerializeField] private float _linear_displacement = 0.1f;

        public override float GetHeight(Vector2 position)
        {
            return SeaHeight - _linear_displacement * Mathf.Abs(GetOffsetsFromCoastline(position));
        }
    }
}