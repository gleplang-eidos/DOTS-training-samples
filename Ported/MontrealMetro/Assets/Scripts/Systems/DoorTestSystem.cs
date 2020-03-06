using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class DoorTestSystem : JobComponentSystem
{
    float timer = 0f;

    protected override void OnCreate()
    {
        timer = 10f;
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return inputDeps;

        timer -= Time.DeltaTime;

        var jobHandle = inputDeps;
        if (timer < 0f)
        {
            timer = 5f;
            Entities.
            WithStructuralChanges().
            WithoutBurst().
            WithNone<DockedTag>().
            ForEach((Entity e, TrainComponent train, ref DestinationComponent destination, in LocalToWorld localToWorld) =>
            {
                for (int i = 0; i < train.Wagons.Length; ++i)
                {
                    EntityManager.AddComponent<DockedTag>(train.Wagons[i]);
                }
                
            }).Run();

            Entities.WithStructuralChanges().WithoutBurst().ForEach((Entity entity, ref CommuterComponent commuterComponent, ref CommuterWagonComponent commuterWagonComponent) =>
            {
                if (commuterComponent.isAtTargetPosition && commuterComponent.targetPosition.Equals(commuterWagonComponent.SeatPosition))
                {
                    commuterWagonComponent.BoardingState = CommuterWagonComponent.EBoardingState.Unboarding;
                }
            }).Run();
        }       

        return jobHandle;
    }
}
