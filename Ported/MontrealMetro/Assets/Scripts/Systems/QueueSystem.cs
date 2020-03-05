using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(AssignQueueSystem))]
public class QueueSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref Translation translation, ref CommuterComponent commuterComponent, ref CommuterQueueComponent commuterQueueComponent) =>
            {
                var queueComponent = EntityManager.GetComponentData<QueueComponent>(commuterQueueComponent.Queue);

                commuterComponent.targetPosition = queueComponent.Position;
            }
        ).Run();

        return inputDeps;
    }
}
