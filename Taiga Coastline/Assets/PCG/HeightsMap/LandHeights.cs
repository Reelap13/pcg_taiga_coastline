using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG_Map.Heights
{
    public class LandHeights : HeightsProcessor
    {
        [SerializeField] private float _scale;
        [SerializeField] private float _height_coefficient = 50;
        [SerializeField] private float _linear_displacement = 0.01f;

        public override float GetHeight(Vector2 position)
        {


            return SeaHeight + _height_coefficient * 
                Mathf.PerlinNoise(position.x / _scale, position.y / _scale) * 
                Mathf.Min(1, _linear_displacement * GetOffsetsFromCoastline(position));
        }
    }
}