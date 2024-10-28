using PCG_Map.Algorithms;
using PCG_Map.Algorithms.Linear;
using PCG_Map.Algorithms.PerlinNoise;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PCG_Map.New_Bioms.Creator
{
    public class HeightsMapCreator : MonoBehaviour
    {
        public HeightsMapAlgorithmType Type;

        public PerlinNoiseAlgorithmCreatorData PerlinNoiseData;
        public LinearFunctionCreatorData LinearData;

        public IHeightsMapAlgorithm GetHeightsMapAlgorithm()
        {
            IHeightsMapAlgorithm algorithm = null;
            switch (Type)
            {
                case HeightsMapAlgorithmType.PERLIN_NOISE:
                    algorithm = new PerlinNoiseAlgorithm() { Data = new(PerlinNoiseData) };
                    break;
                case HeightsMapAlgorithmType.LINEAR:
                    algorithm = new LinearFunction() { Data = new(LinearData) };
                    break;
            }

            return algorithm;
        }
    }

    [CustomEditor(typeof(HeightsMapCreator))]
    public class HeightsMapCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            HeightsMapCreator creator = (HeightsMapCreator)target;

            creator.Type = (HeightsMapAlgorithmType)EditorGUILayout.EnumPopup("Selected Algorithm", creator.Type);

            switch (creator.Type)
            {
                case HeightsMapAlgorithmType.PERLIN_NOISE:
                    EditorGUILayout.LabelField("Voronoi Algorithm Settings");
                    creator.PerlinNoiseData.BaseValue = EditorGUILayout.FloatField("Base Value", creator.PerlinNoiseData.BaseValue);
                    creator.PerlinNoiseData.Smoothness = EditorGUILayout.FloatField("Smoothness Value", creator.PerlinNoiseData.Smoothness);
                    creator.PerlinNoiseData.Amplitude = EditorGUILayout.FloatField("Amplitude Value", creator.PerlinNoiseData.Amplitude);
                    creator.PerlinNoiseData.Borders = EditorGUILayout.Vector2Field("Borders Value", creator.PerlinNoiseData.Borders);
                    break;
                case HeightsMapAlgorithmType.LINEAR:
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