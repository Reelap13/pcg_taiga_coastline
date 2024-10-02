using PCG_Map.Heights;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms.Area
{
    public class DistributionByDistanceFromCoastline : BiomAreaController
    {
        [SerializeField] private Vector2 _area_interval;
        public override bool IsInBiom(Vector2 position)
        {
            float offset = HeightsAgent.Instance.GetOffsetsFromCoastline(position);
            return _area_interval.x <= offset && offset <= _area_interval.y;
        }
    }
}