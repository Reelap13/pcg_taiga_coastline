using System.Collections;
using Unity.Collections;
using UnityEngine;

namespace PCG_Map.Heights
{
    public class CoastlineManager : MonoBehaviour
    {
        [SerializeField] private CoastlineGenerator _coastlineGenerator;
        private CoastlineSearcher _coastlineSearcher;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            _coastlineGenerator.Initialize();

            NativeArray<Vector2> points = _coastlineGenerator.GetPoints();
            _coastlineSearcher = new CoastlineSearcher(points);
        }

        public float GetYValue(float x)
        {
            float[] x_array = new float[1]{ x };
            return GetYValues(x_array)[0];
        }

        public float[] GetYValues(float[] x_values)
        {
            NativeArray<float> x_array = new NativeArray<float>(x_values, Allocator.TempJob);
            NativeArray<float> y_array = _coastlineSearcher.GetYValues(x_array);

            float[] y_values = y_array.ToArray();

            x_array.Dispose();
            y_array.Dispose();

            return y_values;
        }
    }
}
