using PCG_Map.Algorithms;
using PCG_Map.Algorithms.Voronoi;
using PCG_Map.Heights;
using PCG_Map.New_Bioms.Creator;
using PCG_Map.Objects;
using PCG_Map.Textures;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static PCG_Map.Heights.CoastlineSearcher;

namespace PCG_Map.New_Bioms
{
    public class BiomsController : Singleton<BiomsController>
    {
        [SerializeField] private int _centers_number = 100;
        [SerializeField] private Vector4 _borders = new Vector4(-1000, -1000, 1000, 1000);
        [SerializeField] private float _seed;

        private BiomCreator[] _bioms_creators;
        private BiomTemplate[] _bioms_templates;
        private NativeArray<Biom> _bioms;
        private VoronoiAlgorithm _algorithm;

        private int _bioms_templates_objects_number;

        public void Initialize()
        {
            LoadBiomsTemplates();
            GenerateBioms();
            PreprocessingDataToVoronoiAlgorithm();
        }

        public void RegisterBioms()
        {

            for (int i = 0; i < _bioms_templates.Length; ++i)
            {
                HeightsAgent.Instance.RegisterBiomTemplate(i,
                    _bioms_templates[i].HeightsMapAlgorithmType,
                    _bioms_creators[i].GetHeightsMapAlgorithm());
                TexturesAgent.Instance.RegisterBiomTemplate(i,
                    _bioms_templates[i].Texture);
                ObjectsAgent.Instance.RegisterBiomTemplate(i,
                    _bioms_templates[i].Objects);
            }
        }

        private void LoadBiomsTemplates()
        {
            _bioms_creators = GetComponentsInChildren<BiomCreator>();
            _bioms_templates = new BiomTemplate[_bioms_creators.Length];

            _bioms_templates_objects_number = 0;

            for (int i = 0; i < _bioms_creators.Length; ++i)
            {
                BiomTemplate template = _bioms_creators[i].GetBiomTemplate();
                template.ID = i;
                _bioms_templates[i] = template;

                _bioms_templates_objects_number += template.Objects.Length;
            }
        }

        private void GenerateBioms()
        {
            _bioms = new NativeArray<Biom>( _centers_number, Allocator.Persistent);

            Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)_seed);

            for (int i = 0; i < _centers_number; ++i)
            {
                float2 position = new float2(
                    random.NextFloat(_borders.x, _borders.z), 
                    random.NextFloat(_borders.y, _borders.w));
                BiomTemplate template = _bioms_templates[random.NextInt(0, _bioms_templates.Length)];

                _bioms[i] = new Biom() {
                    TemplateID = template.ID,
                    Type = template.Type,
                    Position = position
                };
            }
        }

        private void PreprocessingDataToVoronoiAlgorithm()
        {
            NativeArray<float2> centers = new NativeArray<float2>(_centers_number, Allocator.Persistent);
            for (int i = 0; i < _centers_number; ++i)
            {
                centers[i] = _bioms[i].Position;
                //Debug.Log($"{i} : {centers[i]}");
            }

            VoronoiAlgorithmData data = new VoronoiAlgorithmData() { Borders = _borders, Centers = centers };
            _algorithm = new VoronoiAlgorithm() { Data = data };
        }

        public FindCenters GetBioms(float2 start_position, int size, float step, NativeArray<PointBiom> bioms_id)
        {
            var job = new FindCenters
            {
                StartPosition = start_position,
                Size = size,
                Step = step,
                Bioms = _bioms,
                Algorithm = _algorithm,
                BiomsID = bioms_id
            };

            return job;
            /*JobHandle jobHandle = job.Schedule(size * size, 64);
            jobHandle.Complete();

            return bioms_id;*/
        }

        private void OnDestroy()
        {
            foreach (var biom_template in _bioms_templates)
                biom_template.Dispose();
            _bioms.Dispose();
            _algorithm.Dispose();
        }

        public int BiomsTemplatesNumber { get { return _bioms_templates.Length; } }
        public int BiomsTemplatesObjectsNumber { get { return _bioms_templates_objects_number; } }
    }

    [BurstCompile]
    public struct FindCenters : IJobParallelFor
    {
        [ReadOnly] public float2 StartPosition;
        [ReadOnly] public int Size;
        [ReadOnly] public float Step;
        [ReadOnly] public NativeArray<Biom> Bioms;
        [ReadOnly] public VoronoiAlgorithm Algorithm;
        public NativeArray<PointBiom> BiomsID;

        public void Execute(int index)
        {
            int i = index / Size;
            int j = index % Size;

            float2 position = StartPosition + new float2(i, j) * Step;
            Algorithm.Calculate2NearestCenters(position, 
                out int first, out int second,
                out float first_distance, out float second_distance);

            BiomsID[index] = new PointBiom
            {
                TemplateID = Bioms[first].TemplateID,
                BiomCoefficient = FindBiomCoefficient(first, second, first_distance, second_distance)
            };
        }


        private float FindBiomCoefficient(int first, int second, float first_distance, float second_distance)
        {
            Biom first_biom = Bioms[first];
            Biom second_biom = Bioms[second];

            //if (first_biom.Type == second_biom.Type)
            //    return 1.0f;

            float coefficient = 1 - first_distance / (first_distance + second_distance); // from 0.5 to 1
            coefficient = (coefficient - 0.5f) * 2; // from 0 to 1

            return coefficient;
        }
        /* Find closest center
        public void Execute(int index)
        {
            int i = index / Size;
            int j = index % Size;

            int biom_id = Algorithm.GetBiom(StartPosition + new float2(i, j) * Step);

            BiomsID[index] = Bioms[biom_id].TemplateID;
        }*/
    }

    public struct PointBiom
    {
        public int TemplateID;
        public float BiomCoefficient;
    }
}