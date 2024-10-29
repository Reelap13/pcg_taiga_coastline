using PCG_Map.Algorithms;
using PCG_Map.Algorithms.Voronoi;
using PCG_Map.Heights;
using PCG_Map.New_Bioms.Creator;
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

        private NativeArray<BiomTemplate> _bioms_templates;
        private NativeArray<Biom> _bioms;
        private VoronoiAlgorithm _algorithm;

        public void Initialize()
        {
            LoadBiomsTemplates();
            GenerateBioms();
            PreprocessingDataToVoronoiAlgorithm();
        }

        private void LoadBiomsTemplates()
        {
            BiomCreator[] bioms_creators = GetComponentsInChildren<BiomCreator>();
            _bioms_templates = new NativeArray<BiomTemplate>(bioms_creators.Length, Allocator.Persistent);

            for (int i = 0; i < bioms_creators.Length; ++i)
            {
                BiomTemplate template = bioms_creators[i].GetBiomTemplate();
                template.ID = i;
                _bioms_templates[i] = template;

                HeightsAgent.Instance.RegisterBiomTemplate(i, 
                    _bioms_templates[i].HeightsMapAlgorithmType,
                    bioms_creators[i].GetHeightsMapAlgorithm());
                TexturesAgent.Instance.RegisterBiomTemplate(i, 
                    _bioms_templates[i].TextureID);
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
                _bioms[i] = new Biom() {
                    TemplateID = random.NextInt(0, _bioms_templates.Length), 
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

        public FindCenters<VoronoiAlgorithm> GetBioms(float2 start_position, int size, float step, NativeArray<int> bioms_id)
        {
            var job = new FindCenters<VoronoiAlgorithm>
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
            _bioms_templates.Dispose();
            _bioms.Dispose();
            _algorithm.Dispose();
        }
    }

    [BurstCompile]
    public struct FindCenters<T> : IJobParallelFor where T: struct, IBiomsDistibutionsAlgorithm
    {
        [ReadOnly] public float2 StartPosition;
        [ReadOnly] public int Size;
        [ReadOnly] public float Step;
        [ReadOnly] public NativeArray<Biom> Bioms;
        [ReadOnly] public T Algorithm;
        public NativeArray<int> BiomsID;

        public void Execute(int index)
        {
            int i = index / Size;
            int j = index % Size;

            int biom_id = Algorithm.GetBiom(StartPosition + new float2(i, j) * Step);

            BiomsID[index] = Bioms[biom_id].TemplateID;
        }
    }
}