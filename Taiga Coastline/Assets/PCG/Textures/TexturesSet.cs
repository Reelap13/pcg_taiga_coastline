using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Textures
{
    public class TexturesSet : Singleton<TexturesSet>
    {
        [SerializeField] private Texture2D[] _textures;

        private Dictionary<int, TextureSetData> _id_to_texture;
        private Dictionary<Texture2D, TextureSetData> _texture_to_id;

        public void Initialize()
        {
            int id = 0;

            _id_to_texture = new();
            _texture_to_id = new();

            foreach (Texture2D texture in _textures)
            {
                TextureSetData data = new(texture, id);
                _id_to_texture.Add(id, data);
                _texture_to_id.Add(texture, data);

                ++id;
            }
        }

        public TextureSetData GetTextureData(int id)
        {
            if (IsExisted(id))
                return _id_to_texture[id];

            Debug.Log($"Texture with id {id} wasn't found in the registed textures");
            return null;
        }

        public TextureSetData GetTextureData(Texture2D texture)
        {
            if (IsExisted(texture))
                return _texture_to_id[texture];

            Debug.Log($"Texture with name {texture.name} wasn't found in the registed textures");
            return null;
        }

        private bool IsExisted(int id) {  return _id_to_texture.ContainsKey(id); }
        private bool IsExisted(Texture2D texture) {  return _texture_to_id.ContainsKey(texture); }
        public int TexturesNumber { get { return _textures.Length; } }
    }
}