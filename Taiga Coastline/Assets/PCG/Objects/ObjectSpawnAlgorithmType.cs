using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Objects
{
    [System.Serializable]
    public enum ObjectSpawnAlgorithmType
    {
        BY_SPAWN_FREQUENCY,
        MULIPLE_BY_SPAWN_FREQUENCY,
        ALWAYS_ONE,
        ALWAIS_ONE_PER_CHUNK
    }
}