using PCG_Map.New_Bioms;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace PCG_Map.Objects
{
    public class ObjectsAgent : Singleton<ObjectsAgent>
    {
        private ObjectsAlgorithmData _algorithm_data;

        private BiomsController BiomsController => Generator.Instance.Bioms;

        public FindObjects GetObjects(float2 start_position, int size, float step,
            NativeArray<int> bioms_id, NativeList<ObjectData> objects)
        {
            var job = new FindObjects
            {
                StartPosition = start_position,
                Size = size,
                Step = step,
                Seed = Generator.Instance.Seed,
                AlgorithmData = _algorithm_data,
                BiomsId = bioms_id,
                Objects = objects.AsParallelWriter()
            };

            return job;
        }

        public void Initialize()
        {
            NativeArray<ObjectPrefabData> prefabs = new NativeArray<ObjectPrefabData>(BiomsController.BiomsTemplatesObjectsNumber, Allocator.Persistent);
            NativeHashMap<int, int2> links_to_prefab = new NativeHashMap<int, int2>(BiomsController.BiomsTemplatesNumber, Allocator.Persistent);

            _algorithm_data = new ObjectsAlgorithmData(prefabs, links_to_prefab);
        }

        public void RegisterBiomTemplate(int biom_template_id, NativeArray<ObjectPrefabData> prefabs)
        {
            _algorithm_data.RegisterBiomTemplate(biom_template_id, prefabs);
        }

        private void OnDestroy()
        {
            _algorithm_data.Dispose();
        }
    }

    [BurstCompile]
    public struct FindObjects : IJob
    {
        [ReadOnly] public float2 StartPosition;
        [ReadOnly] public int Size;
        [ReadOnly] public float Step;
        [ReadOnly] public int Seed;
        [ReadOnly] public ObjectsAlgorithmData AlgorithmData;
        [ReadOnly] public NativeArray<int> BiomsId;
        [WriteOnly] public NativeList<ObjectData>.ParallelWriter Objects;

        public void Execute()
        {
            var random = new Unity.Mathematics.Random((uint)Seed + (uint)(StartPosition.x * Size) + (uint)(StartPosition.y * Step));

            for (int x = 0; x < Size; ++x)
            {
                for (int y = 0; y < Size; ++y)
                {
                    float2 position = StartPosition + new float2(x, y) * Step;
                    NativeArray<ObjectPrefabData> objects = AlgorithmData.GetBiomObjects(BiomsId[x * Size + y]);

                    foreach (ObjectPrefabData obj in objects)
                        ProcessObjectPrefab(obj, position, ref random);

                    objects.Dispose();
                }
            }
        }

        private void ProcessObjectPrefab(ObjectPrefabData obj_prefab, float2 position, ref Unity.Mathematics.Random random)
        {
            if (!IsSpawnable(obj_prefab, ref random))
                return;

            Objects.AddNoResize(new ObjectData
            {
                PrefabID = obj_prefab.PrefabID,
                Position = GetObjectPosition(obj_prefab, position, ref random),
                Height = GetObjectHeight(obj_prefab),
                Rotation = GetRotateion(random)
            });
        }

        private bool IsSpawnable(ObjectPrefabData obj_prefab, ref Unity.Mathematics.Random random)
        {
            switch (obj_prefab.SpawnAlgorithmType)
            {
                case ObjectSpawnAlgorithmType.ALWAYS_ONE: return true;
                case ObjectSpawnAlgorithmType.BY_SPAWN_FREQUENCY: return random.NextFloat() > (1 - obj_prefab.SpawnFrequency); 
            }

            return false;
        }

        private float2 GetObjectPosition(ObjectPrefabData obj_prefab, float2 position, ref Unity.Mathematics.Random random)
        {
            switch (obj_prefab.PlacementAlgorithmType)
            {
                case ObjectPlacementAlgorithmType.STATIC: return position;
                case ObjectPlacementAlgorithmType.STEP_AREA_CENTER: return position + Step / 2;
                case ObjectPlacementAlgorithmType.RANDOM_POSITION_IN_STEP_AREA: return position + random.NextFloat2() * Step;
            }

            return new();
        }

        private float GetObjectHeight(ObjectPrefabData obj_prefab)
        {
            return obj_prefab.Height + obj_prefab.HeightOffset;
        }

        private float GetRotateion(Unity.Mathematics.Random random)
        {
            return random.NextFloat() * 360;
        }
    }
}