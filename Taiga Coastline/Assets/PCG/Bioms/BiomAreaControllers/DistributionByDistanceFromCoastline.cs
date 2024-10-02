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

        public float GetDistanceFromBoarders(Vector2 position)
        {
            if (!IsInBiom(position))
                return -1;

            float offset = HeightsAgent.Instance.GetOffsetsFromCoastline(position);
            return Mathf.Max(offset - _area_interval.x, _area_interval.y - offset);
        }
    }
}