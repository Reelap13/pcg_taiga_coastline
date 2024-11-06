using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Objects
{
    [System.Serializable]
    public enum ObjectPlacementAlgorithmType
    {
        STATIC,
        STEP_AREA_CENTER,
        RANDOM_POSITION_IN_STEP_AREA
    }
}