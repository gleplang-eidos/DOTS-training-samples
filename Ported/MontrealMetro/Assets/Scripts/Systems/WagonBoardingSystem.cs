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
        var ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, ref Translation translation, ref CommuterComponent commuterComponent, ref CommuterWagonComponent commuterWagonComponent) =>
            {
                if(commuterWagonComponent.BoardingState == CommuterWagonComponent.EBoardingState.Boarding)
                {
                    if (!commuterComponent.targetPosition.Equals(commuterWagonComponent.EntryPosition) &&
                    !commuterComponent.targetPosition.Equals(commuterWagonComponent.SeatPosition))
                    {
                        commuterComponent.targetPosition = commuterWagonComponent.EntryPosition;
                        commuterComponent.isAtTargetPosition = false;
                    }

                    if (commuterComponent.isAtTargetPosition &&
                       commuterComponent.targetPosition.Equals(commuterWagonComponent.EntryPosition))
                    {
                        commuterComponent.targetPosition = commuterWagonComponent.SeatPosition;
                        commuterComponent.isAtTargetPosition = false;
                    }

                    if (commuterComponent.isAtTargetPosition &&
                       commuterComponent.targetPosition.Equals(commuterWagonComponent.SeatPosition))
                    {
                        ecb.AddComponent(entity, new Parent() { Value = commuterWagonComponent.Seat });
                        ecb.AddComponent<LocalToParent>(entity);
                        ecb.SetComponent(entity, new Translation());
                    }
                }
                else
                {
                    if(commuterComponent.targetPosition.Equals(commuterWagonComponent.SeatPosition))
                    {
                        commuterComponent.targetPosition = commuterWagonComponent.EntryPosition;
                        commuterComponent.isAtTargetPosition = false;
                    }

                    if (commuterComponent.isAtTargetPosition &&
                       commuterComponent.targetPosition.Equals(commuterWagonComponent.EntryPosition))
                    {
                        commuterComponent.targetPosition = commuterWagonComponent.ExitPosition;
                        commuterComponent.isAtTargetPosition = false;
                    }

                    if (commuterComponent.isAtTargetPosition &&
                       commuterComponent.targetPosition.Equals(commuterWagonComponent.ExitPosition))
                    {
                        ecb.RemoveComponent<CommuterWagonComponent>(entity);
                        ecb.AddComponent<UnassignedCommuterTag>(entity);
                    }
                }
                
            }
        ).Run();

        return inputDeps;
    }
}
