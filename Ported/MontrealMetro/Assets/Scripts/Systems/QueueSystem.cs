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

                var commuters = EntityManager.GetBuffer<CommuterBufferElementData>(commuterQueueComponent.Queue);

                int queueIndex = -1;
                for(int i = 0; i < commuters.Length; i++)
                {
                    if(commuters[i].entity == entity)
                    {
                        queueIndex = i;
                        break;
                    }
                }

                if(queueIndex == -1)
                {
                    queueIndex = commuters.Add(new CommuterBufferElementData { entity = entity } );
                }

                //EntityManager.SetComponentData(entity, new Translation { Value = queueComponent.Position });
            }
        ).Run();

        return inputDeps;
    }
}
