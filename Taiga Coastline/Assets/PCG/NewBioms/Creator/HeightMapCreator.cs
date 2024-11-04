using PCG_Map.Algorithms;
using PCG_Map.Algorithms.Linear;
using PCG_Map.Algorithms.PerlinNoise;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PCG_Map.New_Bioms.Creator
{
    public class HeightMapCreator : MonoBehaviour
    {
        public HeightMapAlgorithmType Type;

        public PerlinNoiseAlgorithmCreatorData PerlinNoiseData;
        public MultiPerlinNoiseAlgorithmCreatorData MultiPerlinNoiseData;
        public LinearFunctionCreatorData LinearData;

        public IHeightMapAlgorithm GetHeightsMapAlgorithm()
        {
            IHeightMapAlgorithm algorithm = null;
            switch (Type)
            {
                case HeightMapAlgorithmType.PERLIN_NOISE:
                    algorithm = new PerlinNoiseAlgorithm() { Data = new(PerlinNoiseData) };
                    break;
                case HeightMapAlgorithmType.MULTI_PERLIN_NOISE:
                    algorithm = new MultiPerlinNoiseAlgorithm() { Data = new(MultiPerlinNoiseData) };
                    break;
                case HeightMapAlgorithmType.LINEAR:
                    algorithm = new LinearFunction() { Data = new(LinearData) };
                    break;
            }

            return algorithm;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(HeightMapCreator))]
    public class HeightsMapCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            HeightMapCreator creator = (HeightMapCreator)target;

            creator.Type = (HeightMapAlgorithmType)EditorGUILayout.EnumPopup("Selected Algorithm", creator.Type);

            switch (creator.Type)
            {
                case HeightMapAlgorithmType.PERLIN_NOISE:
                    EditorGUILayout.LabelField("Voronoi Algorithm Settings");
                    creator.PerlinNoiseData.BaseValue = EditorGUILayout.FloatField("Base Value", creator.PerlinNoiseData.BaseValue);
                    creator.PerlinNoiseData.Smoothness = EditorGUILayout.FloatField("Smoothness Value", creator.PerlinNoiseData.Smoothness);
                    creator.PerlinNoiseData.Amplitude = EditorGUILayout.FloatField("Amplitude Value", creator.PerlinNoiseData.Amplitude);
                    creator.PerlinNoiseData.Borders = EditorGUILayout.Vector2Field("Borders Value", creator.PerlinNoiseData.Borders);

                    creator.PerlinNoiseData.CoefficientEffect = (CoefficientEffectType)EditorGUILayout.EnumPopup("Coefficient Effect Type", creator.PerlinNoiseData.CoefficientEffect);
                    
                    switch (creator.PerlinNoiseData.CoefficientEffect)
                    {
                        case CoefficientEffectType.IGNORE:
                            break;
                        default:
                            creator.PerlinNoiseData.Coefficient = EditorGUILayout.FloatField("Coefficient Value", creator.PerlinNoiseData.Coefficient);
                            break;
                    }


                    break;
                case HeightMapAlgorithmType.LINEAR:
                    EditorGUILayout.LabelField("Linear Function Settings");
                    creator.LinearData.BaseValue = EditorGUILayout.FloatField("Base Value", creator.LinearData.BaseValue);
                    creator.LinearData.Coefficients = EditorGUILayout.Vector2Field("Coefficients Value", creator.LinearData.Coefficients);
                    creator.LinearData.Offsets = EditorGUILayout.Vector2Field("Offsets Value", creator.LinearData.Offsets);
                    creator.LinearData.Borders = EditorGUILayout.Vector2Field("Borders Value", creator.LinearData.Borders);
                    break;
            }

            if (GUI.changed)
                EditorUtility.SetDirty(creator);
        }
    }
#endif
}