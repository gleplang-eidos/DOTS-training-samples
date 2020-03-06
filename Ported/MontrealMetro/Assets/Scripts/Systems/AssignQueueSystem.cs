using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;

[UpdateAfter(typeof(DockSystem))]
public class AssignQueueSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var random = new Unity.Mathematics.Random((uint)Random.Range(0, int.MaxValue));
        Entities.WithStructuralChanges().WithAll<UnassignedCommuterTag>().ForEach((Entity e, CommuterPlatformComponent commuter) =>
        {
            var queues = EntityManager.GetBuffer<QueueBufferElementData>(commuter.PlatformEntity);
            EntityManager.AddComponentData<CommuterQueueComponent>(e, new CommuterQueueComponent { Queue = queues[random.NextInt(0, queues.Length)].entity });
            EntityManager.RemoveComponent<UnassignedCommuterTag>(e);

        }).Run();   
        
        return default;
    }
}
