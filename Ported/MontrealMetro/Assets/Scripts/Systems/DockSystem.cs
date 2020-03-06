using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(WagonMovementSystem))]
public class DockSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    { 
        var railComponents = GetComponentDataFromEntity<RailComponent>(true);
        var localToWorlds = GetComponentDataFromEntity<LocalToWorld>(true);
        var doorPanels = GetComponentDataFromEntity<DoorPanelComponent>();
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

        Entities.
            WithReadOnly(localToWorlds).
            WithReadOnly(railComponents).
            WithStructuralChanges().
            WithoutBurst().
            WithNone<DockedTag>().
            ForEach((Entity e, TrainComponent train, ref DestinationComponent destination, in LocalToWorld localToWorld) =>
            {
                bool destinationIsPlatformStart = false;
                if (railComponents.HasComponent(destination.Target))
                {
                    destinationIsPlatformStart = railComponents[destination.Target].Type == RailMarkerType.PLATFORM_END;
                }

                // If close enough to a dock entry, stop every wagon by adding the docked tag.
                if (destinationIsPlatformStart && math.abs(math.length(localToWorlds[destination.Target].Position - localToWorld.Position)) <= 1)
                {
                    // Move this for loop at the end of the scope to raise the exception.
                    //Open doors
                    for (var i = 0; i < train.Doors.Length; ++i)
                    {
                        var door = train.Doors[i];
                        if (doorPanels.HasComponent(door))
                        {
                            var doorPanelComponent = doorPanels[door];
                            doorPanelComponent.DoorState = DoorState.Opening;
                            doorPanels[door] = doorPanelComponent;
                        }
                    }

                    for (var i = 0; i < train.Wagons.Length; ++i)
                    {
                        unsafe
                        {
                            ecb.AddComponent<DockedTag>(train.Wagons[i]);
                        }
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        return inputDeps;
    }
}
