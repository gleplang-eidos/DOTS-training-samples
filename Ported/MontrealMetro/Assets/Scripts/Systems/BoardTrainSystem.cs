using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;

[UpdateAfter(typeof(QueueSystem))]
public class BoardTrainSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var random = new Unity.Mathematics.Random((uint)Random.Range(0, int.MaxValue));

        var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = ecbSystem.CreateCommandBuffer();

        var wagonComponents = GetComponentDataFromEntity<WagonComponent>(true);
        var queues = GetBufferFromEntity<QueueBufferElementData>(true);
        var commuters = GetBufferFromEntity<CommuterBufferElementData>();
        var localToWorlds = GetComponentDataFromEntity<LocalToWorld>(true);
        var commuterWagonComponents = GetComponentDataFromEntity<CommuterWagonComponent>(true);
        var queueComponents = GetComponentDataFromEntity<QueueComponent>(true);

        Entities.WithoutBurst().WithAll<DockedTag>().ForEach((Entity e, TrainComponent train) =>
        {
            
            for (int i = 0; i < train.Wagons.Length; ++i)
            {
                var wagonComponent = wagonComponents[train.Wagons[i]];
                var queueInstance = queues[train.Platform];
                var commuterInstance = commuters[queueInstance[i].entity];

                for (int j = 0; j < commuterInstance.Length; ++j)
                {
                    // already assigned a seat or no more seats left
                    if (commuterWagonComponents.Exists(commuterInstance[j].entity) || j >= wagonComponent.Seats.Length)
                    {
                        continue;
                    }

                    var commuterComponent = new CommuterWagonComponent();
                    var seat = wagonComponent.Seats[j];
                  
                    commuterComponent.SeatPosition = localToWorlds[seat].Position;
                    commuterComponent.EntryPosition = localToWorlds[wagonComponent.LeftEntryEntity].Position;
                    commuterComponent.ExitPosition = localToWorlds[queueInstance[i].entity].Position;
                    commuterComponent.BoardingState = CommuterWagonComponent.EBoardingState.Boarding;
                    ecb.AddComponent(commuterInstance[j].entity, commuterComponent);
                    ecb.RemoveComponent<CommuterQueueComponent>(commuterInstance[j].entity);
                    commuterInstance.RemoveAt(j);
                }
            }

            

        }).Run();
        
        return default;
    }
}
