using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(QueueSystem))]
public class WagonBoardingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref Translation translation, ref CommuterComponent commuterComponent, ref CommuterWagonComponent commuterWagonComponent) =>
            {
                if(!commuterComponent.targetPosition.Equals(commuterWagonComponent.EntryPosition) &&
                    !commuterComponent.targetPosition.Equals(commuterWagonComponent.SeatPosition))
                {
                    commuterComponent.targetPosition = commuterWagonComponent.EntryPosition;
                    commuterComponent.isAtTargetPosition = false;
                }

                if(commuterComponent.isAtTargetPosition &&
                   commuterComponent.targetPosition.Equals(commuterWagonComponent.EntryPosition))
                {
                    commuterComponent.targetPosition = commuterWagonComponent.SeatPosition;
                    commuterComponent.isAtTargetPosition = false;
                }
            }
        ).Run();

        return inputDeps;
    }
}
