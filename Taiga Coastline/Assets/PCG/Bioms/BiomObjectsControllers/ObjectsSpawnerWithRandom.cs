using PCG_Map.Bioms.Area;
using PCG_Map.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms.Objects
{
    public class ObjectsSpawnerWithRandom : BiomObjectsController
    {
        [SerializeField] private DistributionByDistanceFromCoastline _area_controller;
        [SerializeField] private BiomObjectData[] _objects_prefabs;
        [SerializeField] private float _min_distance_from_boarders;

        public override List<OldObjectData> GetObjectsPrefabs(Vector2 position)
        {
            List<OldObjectData> objects = new();

            Random.InitState((int)(Generator.Instance.Seed + position.x * 122 + position.y * 145 + Mathf.Pow(position.x + 1, 2) + Mathf.Pow(position.y + 1, 3)));
            float coefficient = GetDistanceCoefficient(position);            

            foreach (BiomObjectData obj in _objects_prefabs)
            {
                float rand = Random.value * coefficient;
                for (int i = 1; i < rand / obj.Frequency; ++i)
                    objects.Add(new(obj.Prefab, GetRandomPosition(position), GetRandomRotation()));
            }

            return objects;
        }

        private float GetDistanceCoefficient(Vector2 position)
        {
            float distance = Mathf.Min(1, _area_controller.GetDistanceFromBoarders(position) / _min_distance_from_boarders);
            return Mathf.Pow(distance, 7);
        }
    }
}