using System;
using Unity.Mathematics;

namespace PCG_Map.Objects
{
    public struct ObjectPrefabData : IEquatable<ObjectPrefabData>
    {
        public int PrefabID;
        public ObjectSpawnAlgorithmType SpawnAlgorithmType;
        public ObjectPlacementAlgorithmType PlacementAlgorithmType;
        public ObjectHeightAlgorithmType HeightAlgorithmType;
        public float SpawnFrequency;
        public float Height;
        public float HeightOffset;
        public float2 TerrainHeightBorders;

        public bool Equals(ObjectPrefabData other)
        {
            return PrefabID == other.PrefabID;
        }

        public override bool Equals(object obj)
        {
            return obj is ObjectPrefabData other && Equals(other);
        }
        public override int GetHashCode()
        {
            return PrefabID.GetHashCode();
        }
    }
}
