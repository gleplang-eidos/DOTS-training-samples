using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(WagonMovementSystem))]
public class DockReleaseTrainSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var doorPanels = GetComponentDataFromEntity<DoorPanelComponent>();

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

        Entities.
            WithStructuralChanges().
            WithoutBurst().
            WithNone<TimerComponent>().
            ForEach((Entity e, ref PlatformComponent platform) =>
            {
                if (platform.DockedTrain != Entity.Null)
                {

                    var train = EntityManager.GetSharedComponentData<TrainComponent>(platform.DockedTrain);


                    //Close doors
                    for (var i = 0; i < train.Doors.Length; ++i)
                    {
                        var door = train.Doors[i];
                        if (doorPanels.HasComponent(door))
                        {
                            var doorPanelComponent = doorPanels[door];
                            doorPanelComponent.DoorState = DoorState.Closing;
                            doorPanels[door] = doorPanelComponent;
                        }
                    }

                    // Remove DockedComponent
                    for (var i = 0; i < train.Wagons.Length; ++i)
                    {
                        unsafe
                        {
                            ecb.RemoveComponent<DockedTag>(train.Wagons[i]);
                        }
                    }


                    platform.DockedTrain = Entity.Null;
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        return inputDeps;
    }
}
