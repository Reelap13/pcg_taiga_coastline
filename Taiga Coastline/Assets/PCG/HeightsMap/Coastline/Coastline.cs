using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Heights
{
    public class Coastline : MonoBehaviour
    {
        [SerializeField] private int _iteration_number = 5;
        [SerializeField] private float _height_offset = 1f;
        [SerializeField] private float _roughness = 0.5f;

        [SerializeField] private Vector2 _x_boarders = new(-1000, 1000);
        [SerializeField] private float _y_start = 0;

        [SerializeField] private int _decimal_cach_places = 4;

        [SerializeField] private bool _is_displaying_coastline_by_line = true;

        [SerializeField] private int _coastline_seed_coefficient = 123;

        private Vector2[] _points;
        private Dictionary<float, float> _cached_y_values = new();

        public void Initialize()
        {
            GenerateCoastline();
            if (_is_displaying_coastline_by_line)
                DrawLine();
        }

        private void GenerateCoastline()
        {
            Random.InitState(Generator.Instance.Seed * _coastline_seed_coefficient);

            Vector2[] points = new Vector2[] { new(_x_boarders.x, _y_start), new(_x_boarders.y, _y_start) };

            for (int iteration = 0; iteration < _iteration_number; ++iteration)
            {
                List<Vector2> new_points = new();
                for (int i = 0; i < points.Length - 1; ++i)
                {
                    Vector2 start = points[i];
                    Vector2 end = points[i + 1];
                    new_points.Add(start);

                    Vector2 middle = (start + end) / 2;
                    middle.y += Random.Range(-_height_offset, _height_offset) * Mathf.Pow(_roughness, iteration);

                    new_points.Add(middle);
                }

                new_points.Add(points[points.Length - 1]);
                points = new_points.ToArray();
            }

            _points = points;
        }


        void DrawLine()
        {
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = _points.Length;
            lineRenderer.startWidth = 1f;
            lineRenderer.endWidth = 1f;

            for (int i = 0; i < _points.Length; i++)
            {
                lineRenderer.SetPosition(i, new(_points[i].x, 55, _points[i].y));
            }
        }

        public float GetYValue(float x)
        {
            float rounded_x = (float)System.Math.Round(x, _decimal_cach_places);

            if (_cached_y_values.ContainsKey(rounded_x) && false)
                return _cached_y_values[rounded_x];

            float y = FindYValueByBinarySearch(rounded_x);
            _cached_y_values[rounded_x] = y;
            return y;
        }

        private float FindYValueByBinarySearch(float x)
        {
            int left = 0;
            int right = _points.Length - 1;

            if (x < _points[left].x || x > _points[right].x)
                Debug.LogError($"Try to take coastline point x {x} out the border");

            while (left < right)
            {
                int middle = (left + right) / 2;

                if (_points[middle].x < x)
                {
                    left = middle + 1;
                }
                else
                {
                    right = middle;
                }
            }

            if (left == 0) return _points[0].y;
            if (left == _points.Length) return _points[_points.Length - 1].y;

            Vector2 p1 = _points[left - 1];
            Vector2 p2 = _points[left];

            float t = (x - p1.x) / (p2.x - p1.x);
            return Mathf.Lerp(p1.y, p2.y, t);
        }
    }
}