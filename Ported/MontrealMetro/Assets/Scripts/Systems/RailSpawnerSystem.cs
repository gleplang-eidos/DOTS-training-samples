using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
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
            ref MaterialColor materialColor,
            ref RailSpawnComponent railSpawnComponent
            ) =>
        {
            var rail = EntityManager.Instantiate(railSpawnComponent.RailPrefab);

            EntityManager.SetComponentData(rail, new Translation { Value = translation.Value });
            EntityManager.SetComponentData(rail, new Rotation { Value = rotation.Value });

            EntityManager.AddComponentData(rail, new MaterialColor { Value = materialColor.Value });

            EntityManager.DestroyEntity(e);

        }).Run();

        return default;
    }
}
