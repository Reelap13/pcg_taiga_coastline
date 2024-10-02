using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG_Map.Bioms.Area
{
    public abstract class BiomAreaController : MonoBehaviour
    {
        public abstract bool IsInBiom(Vector2 position);
    }
}