using PCG_Map.Bioms;
using PCG_Map.Chunk;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG_Map.Textures
{
    public class TexturesAgent : Singleton<TexturesAgent>
    {
        [SerializeField] private TexturesSet _textures_set;

        private TextureMapAlgorithmData _algorithm_data;

        public void SetTextures(Vector2 start_position, Vector2Int size, ChunkTexturesData textures_data)
        {
            Vector2 position = new();
            for (int x = 0; x < size.x; x++)
            {
                position.x = (int)start_position.x + x; 
                for (int y = 0; y < size.y; y++)
                {
                    position.y = (int)start_position.y + y;

                    Biom biom = BiomsAgent.Instance.GetBiom(position);
                    TextureData texture_data = _textures_set.GetTextureData(biom.GetTexture(position));

                    textures_data.AddTexture(1, new(x, y), texture_data);
                }
            }
        }
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
            _textures_set.Initialize();

            NativeHashMap<int, int> bioms_textures = new NativeHashMap<int, int>(10, Allocator.Persistent);//10 magic number of bioms count
            _algorithm_data = new TextureMapAlgorithmData()
            {
                BiomsTextures = bioms_textures,
            };
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