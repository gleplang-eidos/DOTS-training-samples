using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class CommutersSpawnerSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithStructuralChanges().ForEach((Entity e, ref Translation translation, ref CommuterSpawnPointComponent commuterSpawnerComponent) =>
        {
            var commuter = EntityManager.Instantiate(commuterSpawnerComponent.CommuterPrefab);
            EntityManager.SetComponentData(commuter, new Translation { Value = translation.Value });

            EntityManager.DestroyEntity(e);
        }).Run();
        return default;
    }
}