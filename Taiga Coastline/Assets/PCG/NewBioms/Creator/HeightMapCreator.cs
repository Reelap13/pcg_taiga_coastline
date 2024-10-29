using PCG_Map.Algorithms;
using PCG_Map.Algorithms.Linear;
using PCG_Map.Algorithms.PerlinNoise;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PCG_Map.New_Bioms.Creator
{
    public class HeightMapCreator : MonoBehaviour
    {
        public HeightMapAlgorithmType Type;

        public PerlinNoiseAlgorithmCreatorData PerlinNoiseData;
        public LinearFunctionCreatorData LinearData;

        public IHeightsMapAlgorithm GetHeightsMapAlgorithm()
        {
            IHeightsMapAlgorithm algorithm = null;
            switch (Type)
            {
                case HeightMapAlgorithmType.PERLIN_NOISE:
                    algorithm = new PerlinNoiseAlgorithm() { Data = new(PerlinNoiseData) };
                    break;
                case HeightMapAlgorithmType.LINEAR:
                    algorithm = new LinearFunction() { Data = new(LinearData) };
                    break;
            }

            return algorithm;
        }
    }

    [CustomEditor(typeof(HeightMapCreator))]
    public class HeightsMapCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
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
}