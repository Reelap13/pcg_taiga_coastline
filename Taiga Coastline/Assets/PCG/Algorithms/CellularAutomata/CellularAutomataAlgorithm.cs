using PCG_Map.New_Bioms;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Algorithms.CelluralAutomata
{
    public class CellularAutomataAlgorithm : MonoBehaviour
    {
        [SerializeField] private int _iteration_number = 5;
        [SerializeField] private int _size = 100;
        [SerializeField] private float2 _borders = new float2(-1000, 1000);
        [SerializeField] private string _path = "Assets/BiomMap.png";
        [SerializeField] private int _draw_coeffitient = 10;

        [SerializeField] private BiomData[] _bioms_data;

        private Map<BiomType> _biom_map;
             
        public void Initialize()
        {
            float2 position = new(_borders.x);
            int size = _size;
            float step = (_borders.y - _borders.x) / size;
            NativeArray<BiomType> biom_map_data = new NativeArray<BiomType>(size * size, Allocator.Persistent);

            NativeArray<BiomData> data = new NativeArray<BiomData>(_bioms_data.Length, Allocator.Persistent);
            for (int i = 0; i < _bioms_data.Length; i++) data[i] = _bioms_data[i];
            CellularAutomataInitialData initial_data = new CellularAutomataInitialData()
            {
                BiomsData = data
            };

            _biom_map = new Map<BiomType>()
            {
                Position = new(position),
                Size = size,
                Step = step,
                Data = biom_map_data
            };

            var job = new CellularAutomataJob()
            {
                StartPosition = position,
                Size = size,
                Step = step,
                IterationNumber = _iteration_number,
                Seed = Generator.Instance.Seed,
                InitialData = initial_data,
                BiomMap = _biom_map
            };

            JobHandle handle = job.Schedule();
            handle.Complete();
            
            data.Dispose();

            DrawBiomMap();
        }

        private void DrawBiomMap()
        {
            Texture2D texture = new Texture2D(_size * _draw_coeffitient, _size * _draw_coeffitient);

            for (int x = 0; x < _size; ++x)
            {
                for (int y = 0; y < _size; ++y)
                {
                    Color color = new();
                    switch(_biom_map.GetData(x, y))
                    {
                        case BiomType.FOREST :
                            color = new Color(34 / 256f, 139 / 256f, 34 / 256f);
                            break;
                        case BiomType.FIELD :
                            color = new Color(124 / 256f, 252 / 256f, 0 / 256f);
                            break;
                        case BiomType.MOUNTAIN :
                            color = new Color(139 / 256f, 137 / 256f, 137 / 256f);
                            break;
                        case BiomType.Lake :
                            color = new Color(70 / 256f, 130 / 256f, 180 / 256f);
                            break;
                    }
                    for (int i = 0; i < _draw_coeffitient; ++i)
                        for (int j = 0; j < _draw_coeffitient; ++j)
                            texture.SetPixel(x * _draw_coeffitient + i, y * _draw_coeffitient + j, color);
                }
            }
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(_path, bytes);
        }

        private void OnDestroy()
        {
            _biom_map.Dispose();
        }

        public BiomType GetBiomType(float2 position)
        {
            return _biom_map.GetData(position);
        }

        public Map<BiomType> BiomMap { get { return _biom_map; } }
    }

    [BurstCompile]
    public struct CellularAutomataJob : IJob
    {
        [ReadOnly] public float2 StartPosition;
        [ReadOnly] public int Size;
        [ReadOnly] public float Step;
        [ReadOnly] public int IterationNumber;
        [ReadOnly] public int Seed;
        [ReadOnly] public CellularAutomataInitialData InitialData;
        public Map<BiomType> BiomMap;

        public void Execute()
        {
            Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)Seed);
            Initialize(ref random);

            for (int iteration = 0; iteration < IterationNumber; ++iteration)
                UpdateMap(ref random);
        }
        private void Initialize(ref Unity.Mathematics.Random random)
        {
            float probablity = 0;
            foreach (BiomData data in InitialData.BiomsData)
                probablity += data.SpawningProbability;

            for (int x = 0; x < Size; ++x)
            {
                for (int y = 0; y < Size; ++y)
                {
                    float value = random.NextFloat(0, probablity);
                    foreach (BiomData data in InitialData.BiomsData)
                    {
                        value -= data.SpawningProbability;
                        if (value < 0)
                        {
                            BiomMap.SetData(data.BiomType, x, y);
                            break;
                        }
                    }
                }
            }
        }

        private void UpdateMap(ref Unity.Mathematics.Random random)
        {
            NativeArray<BiomType> new_biom_map = new NativeArray<BiomType>(BiomMap.Data, Allocator.Temp);

            for (int x = 0; x < Size; ++x)
            {
                for (int y = 0; y < Size; ++y)
                {
                    int2 indexes = new(x, y);
                    BiomData data = GetBiomData(indexes);
                    if (!IsChange(data, ref random))
                        continue;

                    NativeHashMap<int, int> neighbors = GetNeighbors(indexes);

                    new_biom_map[x * Size + y] = GetNewBiomType(neighbors, ref random);

                    neighbors.Dispose();
                }
            }

            NativeArray<BiomType>.Copy(new_biom_map, BiomMap.Data);
            new_biom_map.Dispose();
        }

        private BiomType GetNewBiomType(NativeHashMap<int, int> neighbors, ref Unity.Mathematics.Random random)
        {
            float probability = 0;
            NativeHashMap<int, float> biom_type_probabilities = new NativeHashMap<int, float>(InitialData.BiomsData.Length, Allocator.Temp);
            foreach (BiomData biom in InitialData.BiomsData)
            {
                BiomType biom_type = biom.BiomType;
                float value = biom.ChangingToProbability * neighbors[(int)biom_type] / neighbors.Count;
                probability += value;
                biom_type_probabilities.Add((int)biom_type, value);
            }

            BiomType type = 0;
            float rand_value = random.NextFloat(0, probability);
            foreach (BiomData biom in InitialData.BiomsData)
            {
                rand_value -= biom_type_probabilities[(int)biom.BiomType];
                if (rand_value < 0)
                {
                    type = biom.BiomType;
                    break;
                }
            }

            biom_type_probabilities.Dispose();
            return type;
        }

        private NativeHashMap<int, int> GetNeighbors(int2 indexes)
        {
            NativeHashMap<int, int> neighbors = new NativeHashMap<int, int>(InitialData.BiomsData.Length, Allocator.Temp);
            foreach (BiomData data in InitialData.BiomsData)
                neighbors.Add((int)data.BiomType, 0);

            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    if (i == 0 && j == 0)
                        continue;

                    int x = indexes.x + i;
                    int y = indexes.y + j;

                    if (!BiomMap.IsInMap(x, y))
                        continue;

                    BiomType type = BiomMap.GetData(x, y);
                    neighbors[(int)type] += 1;
                }
            }

            return neighbors;
        }

        private bool IsChange(BiomData data, ref Unity.Mathematics.Random random)
        {
            return data.ChangingFromProbability > random.NextFloat();
        }

        private BiomData GetBiomData(int2 indexes)
        {
            BiomType type = BiomMap.GetData(indexes.x, indexes.y);
            return GetBiomData(type);
        }
        private BiomData GetBiomData(BiomType type)
        {
            BiomData data = InitialData.BiomsData[0];
            foreach (BiomData biom_data in InitialData.BiomsData)
                if (biom_data.BiomType == type)
                {
                    data = biom_data;
                    break;
                }

            return data;
        }
    }

    public struct CellularAutomataInitialData
    {
        public NativeArray<BiomData> BiomsData; 
    }

    [System.Serializable]
    public struct BiomData
    {
        public BiomType BiomType;
        public float SpawningProbability;
        public float ChangingFromProbability;
        public float ChangingToProbability;
    }
}