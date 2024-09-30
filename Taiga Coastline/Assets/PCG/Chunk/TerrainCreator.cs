using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Chunk
{
    public class TerrainCreator : MonoBehaviour
    {
        [SerializeField] private Terrain _terrain_pref;

        public Terrain GenerateTerrain()
        {
            Terrain terrain = Instantiate(_terrain_pref) as Terrain;

            TerrainData original_data = terrain.terrainData;
            TerrainData new_data = Instantiate(original_data);

            terrain.terrainData = new_data;

            return terrain;
        }
    }
}