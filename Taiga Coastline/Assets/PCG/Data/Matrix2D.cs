using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace PCG_Map
{
    public class Matrix2D
    {
        private float[,] _matrix;
        private Vector2Int _size;

        public int Width => _size.x;
        public int Height => _size.y;

        public Matrix2D(int size) : this(new Vector2Int(size, size)) { }
        public Matrix2D(Vector2Int size) : this(size, 0.0f) { }
        public Matrix2D(Vector2Int size, float init_value)
        {
            if (size.x < 0 || size.y < 0)
                Debug.LogError("Matrix2D was initialized with the wrong size(<=0)");

            _matrix = new float[size.x, size.y];
            _size = size;
            SetValue(init_value);
        }

        public void FillWithPerlinNoise(float scale)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    float xCoord = (float)x / Width * scale;
                    float yCoord = (float)y / Height * scale;

                    _matrix[x, y] = Mathf.PerlinNoise(xCoord, yCoord) / 600;
                }
            }
        }

        public void SetValue(float value)
        {
            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    _matrix[i, j] = value;
        }

        public float this[int row, int col]
        {
            get { return _matrix[row, col]; }
            set { _matrix[row, col] = value; }
        }


    }
}