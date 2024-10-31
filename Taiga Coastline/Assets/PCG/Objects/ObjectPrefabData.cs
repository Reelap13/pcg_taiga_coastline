namespace PCG_Map.Objects
{
    public struct ObjectPrefabData
    {
        public int PrefabID;
        public ObjectSpawnAlgorithmType SpawnAlgorithmType;
        public ObjectPlacementAlgorithmType PlacementAlgorithmType;
        public ObjectHeightAlgorithmType HeightAlgorithmType;
        public float SpawnFrequency;
        public float Height;
        public float HeightOffset;
    }
}
