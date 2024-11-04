using PCG_Map.Algorithms;
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
            NativeArray<PointBiom> biom_map, NativeArray<int> texture_map, 
            NativeParallelHashSet<int> textures, Map<float> height_map)
        {
            var job = new FindTextureMap
            {
                StartPosition = start_position,
                Size = size,
                Step = step,
                TerrainHeight = Generator.Instance.HeightsAgent.TerrainHeight,
                AlgorithmData = _algorithm_data,
                BiomMap = biom_map,
                HeightMap = height_map,
                TextureMap = texture_map,
                Textures = textures.AsParallelWriter()
            };

            return job;
        }

        public void RegisterBiomTemplate(int biom_template_id, TextureData texture)
        {
            _algorithm_data.AddBiomTemplateTexture(biom_template_id, texture);
        }

        public void Initialize()
        {
            NativeHashMap<int, TextureData> bioms_textures = new NativeHashMap<int, TextureData>(BiomsController.BiomsTemplatesNumber, Allocator.Persistent);

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
        [ReadOnly] public float TerrainHeight;
        [ReadOnly] public TextureMapAlgorithmData AlgorithmData;
        [ReadOnly] public NativeArray<PointBiom> BiomMap;
        [ReadOnly] public Map<float> HeightMap;
        [WriteOnly] public NativeArray<int> TextureMap;
        [WriteOnly] public NativeParallelHashSet<int>.ParallelWriter Textures;

        public void Execute(int index)
        {
            TextureData texture = AlgorithmData.GetTexture(BiomMap[index].TemplateID);
            int value = GetTextureID(texture, index);
            TextureMap[index] = value;
            Textures.Add(value);
        }

        private int GetTextureID(TextureData texture, int index)
        {
            switch (texture.Type)
            {
                case TextureAlgorithmType.ALWAIS_ONE: return texture.FirstTexture;
                case TextureAlgorithmType.BY_HEIGHT:
                    float height = HeightMap.GetData(index) * TerrainHeight;

                    if (height < texture.FirstMaxHeight) return texture.FirstTexture;
                    if (height < texture.SecondMaxHeight) return texture.SecondTexture;
                    
                    return texture.ThirdTexture;
            }

            return 0;
        }
    }
}