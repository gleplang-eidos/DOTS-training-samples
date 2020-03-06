using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class CommutersSpawnerSystem : JobComponentSystem
{
    int frameCount;
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (++frameCount == 5)
        {
            Entities.WithStructuralChanges().ForEach((Entity e, ref LocalToWorld localToWorld, ref CommuterSpawnPointComponent commuterSpawnerComponent) =>
            {
                UnityEngine.Debug.Log($"SP position: {localToWorld.Position}");
                for (int i = 0; i < commuterSpawnerComponent.NbCommutersToSpawn; i++)
                {
                    var commuter = EntityManager.Instantiate(commuterSpawnerComponent.CommuterPrefab);
                    EntityManager.SetComponentData(commuter, new Translation { Value = localToWorld.Position });
                    EntityManager.AddComponentData(commuter, new CommuterComponent
                    {
                        isAtTargetPosition = true,
                        targetPosition = localToWorld.Position,
                        speed = 2,
                    });

                    EntityManager.AddComponentData(commuter, new CommuterPlatformComponent { PlatformEntity = commuterSpawnerComponent.Platform });
                    EntityManager.AddComponentData(commuter, new UnassignedCommuterTag { });
                }

                EntityManager.DestroyEntity(e);
            }).Run();
            Enabled = false;
        }
        
        return default;
    }
}