using PCG_Map.Chunk;
using PCG_Map.New_Bioms;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Textures
{
    public class TexturesAgent : Singleton<TexturesAgent>
    {
        private TextureMapAlgorithmData _algorithm_data;

        private BiomsController BiomsController => Generator.Instance.Bioms;

        public FindTextureMap GetTextureMap(float2 start_position, int size, float step, 
            NativeArray<int> bioms_id, NativeArray<int> texture_map, NativeParallelHashSet<int> textures)
        {
            var job = new FindTextureMap
            {
                StartPosition = start_position,
                Size = size,
                Step = step,
                AlgorithmData = _algorithm_data,
                BiomsId = bioms_id,
                TextureMap = texture_map,
                Textures = textures.AsParallelWriter()
            };

            return job;
        }

        public void RegisterBiomTemplate(int biom_template_id, int texture_id)
        {
            _algorithm_data.AddBiomTemplateTexture(biom_template_id, texture_id);
        }

        public void Initialize()
        {
            NativeHashMap<int, int> bioms_textures = new NativeHashMap<int, int>(BiomsController.BiomsTemplatesNumber, Allocator.Persistent);
            _algorithm_data = new TextureMapAlgorithmData()
            {
                BiomsTextures = bioms_textures,
            };
        }

        private void OnDestroy()
        {
            _algorithm_data.Dispose();
        }
    }

    [BurstCompile]
    public struct FindTextureMap : IJobParallelFor
    {
        [ReadOnly] public float2 StartPosition;
        [ReadOnly] public int Size;
        [ReadOnly] public float Step;
        [ReadOnly] public TextureMapAlgorithmData AlgorithmData;
        [ReadOnly] public NativeArray<int> BiomsId;
        [WriteOnly] public NativeArray<int> TextureMap;
        [WriteOnly] public NativeParallelHashSet<int>.ParallelWriter Textures;

        public void Execute(int index)
        {
            int value = AlgorithmData.GetTexture(BiomsId[index]);
            TextureMap[index] = value;
            Textures.Add(value);
        }
    }
}