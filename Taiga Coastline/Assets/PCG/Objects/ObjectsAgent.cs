using PCG_Map.Algorithms;
using PCG_Map.New_Bioms;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.UIElements;

namespace PCG_Map.Objects
{
    public class ObjectsAgent : Singleton<ObjectsAgent>
    {
        private ObjectsAlgorithmData _algorithm_data;

        private BiomsController BiomsController => Generator.Instance.Bioms;

        public FindObjects GetObjects(float2 start_position, int size, float step,
            NativeArray<PointBiom> biom_map, NativeList<ObjectData> objects,
            Map<float> height_map)
        {
            var job = new FindObjects
            {
                StartPosition = start_position,
                Size = size,
                Step = step,
                TerrainHeight = Generator.Instance.HeightsAgent.TerrainHeight,
                Seed = Generator.Instance.Seed,
                AlgorithmData = _algorithm_data,
                BiomMap = biom_map,
                HeightMap = height_map,
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
        [ReadOnly] public float TerrainHeight;
        [ReadOnly] public int Seed;
        [ReadOnly] public ObjectsAlgorithmData AlgorithmData;
        [ReadOnly] public NativeArray<PointBiom> BiomMap;
        [ReadOnly] public Map<float> HeightMap;
        [WriteOnly] public NativeList<ObjectData>.ParallelWriter Objects;

        public void Execute()
        {
            var random = new Unity.Mathematics.Random((uint)Seed + (uint)(StartPosition.x * Size) + (uint)(StartPosition.y * Step));

            NativeHashSet<ObjectPrefabData> one_per_chunk_objects = new NativeHashSet<ObjectPrefabData>(10, Allocator.Temp); // 10(magic number)
            for (int x = 0; x < Size; ++x)
            {
                for (int y = 0; y < Size; ++y)
                {
                    float2 position = StartPosition + new float2(x, y) * Step;
                    NativeArray<ObjectPrefabData> objects = AlgorithmData.GetBiomObjects(BiomMap[x * Size + y].TemplateID);

                    foreach (ObjectPrefabData obj in objects)
                        ProcessObjectPrefab(obj, position, ref random, one_per_chunk_objects);

                    objects.Dispose();
                }
            }

            foreach (ObjectPrefabData obj_prefab in one_per_chunk_objects)
            {
                ProcessOneSpawnableObjectPrefab(obj_prefab, ref random);
            }
            one_per_chunk_objects.Dispose();
        }

        private void ProcessObjectPrefab(ObjectPrefabData obj_prefab, float2 position, 
            ref Unity.Mathematics.Random random, NativeHashSet<ObjectPrefabData> one_per_chunk_objects)
        {
            if (!IsSpawnable(obj_prefab, position, ref random, one_per_chunk_objects))
                return;

            Objects.AddNoResize(new ObjectData
            {
                PrefabID = obj_prefab.PrefabID,
                Position = GetObjectPosition(obj_prefab, position, ref random),
                Height = GetObjectHeight(obj_prefab),
                Rotation = GetRotateion(ref random),
                Scale = GetScale(obj_prefab, ref random)
            });
        }

        private void ProcessOneSpawnableObjectPrefab(ObjectPrefabData obj_prefab, ref Unity.Mathematics.Random random)
        {
            float2 position = StartPosition + Step * Size / 2;

            Objects.AddNoResize(new ObjectData
            {
                PrefabID = obj_prefab.PrefabID,
                Position = GetObjectPosition(obj_prefab, position, ref random),
                Height = GetObjectHeight(obj_prefab),
                Rotation = 0,
                Scale = 1f
            });
        }

        private bool IsSpawnable(ObjectPrefabData obj_prefab, float2 position, 
            ref Unity.Mathematics.Random random, NativeHashSet<ObjectPrefabData> one_per_chunk_objects)
        {
            float height = HeightMap.GetData(position) * TerrainHeight;
            if (!(obj_prefab.TerrainHeightBorders.x < height && height < obj_prefab.TerrainHeightBorders.y))
                return false;


            switch (obj_prefab.SpawnAlgorithmType)
            {
                case ObjectSpawnAlgorithmType.ALWAYS_ONE: return true;
                case ObjectSpawnAlgorithmType.BY_SPAWN_FREQUENCY: return random.NextFloat() > (1 - obj_prefab.SpawnFrequency);
                case ObjectSpawnAlgorithmType.ALWAIS_ONE_PER_CHUNK:
                    one_per_chunk_objects.Add(obj_prefab);
                    return false;
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

        private float GetRotateion(ref Unity.Mathematics.Random random)
        {
            return random.NextFloat() * 360;
        }

        private float GetScale(ObjectPrefabData obj_prefab, ref Unity.Mathematics.Random random)
        {
            return random.NextFloat(obj_prefab.ScaleInterval.x, obj_prefab.ScaleInterval.y);
        }
    }
}