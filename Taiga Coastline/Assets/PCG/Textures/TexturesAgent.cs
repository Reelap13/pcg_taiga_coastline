using PCG_Map.Bioms;
using PCG_Map.Chunk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Textures
{
    public class TexturesAgent : MonoBehaviour
    {
        [SerializeField] private TexturesSet _textures_set;

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

        public void Initialize()
        {
            _textures_set.Initialize();
        }
    }
}