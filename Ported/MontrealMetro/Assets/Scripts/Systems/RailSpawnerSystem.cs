using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class RailSpawnerSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithStructuralChanges().ForEach(
            (Entity e,
            ref Translation translation,
            ref Rotation rotation,
            ref RailSpawnComponent railSpawnComponent
            ) =>
        {
            var rail = EntityManager.Instantiate(railSpawnComponent.RailPrefab);

            EntityManager.SetComponentData(rail, new Translation { Value = translation.Value });
            EntityManager.SetComponentData(rail, new Rotation { Value = rotation.Value });

            EntityManager.DestroyEntity(e);

        }).Run();

        return default;
    }
}
