using Unity.Collections;
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
        var lineWaypointComponents = GetComponentDataFromEntity<LineWaypointComponent>(true);
        var localToWorlds = GetComponentDataFromEntity<LocalToWorld>(true);
        var doorPanels = GetComponentDataFromEntity<DoorPanelComponent>();

        Entities.
            WithReadOnly(localToWorlds).
            WithReadOnly(lineWaypointComponents).
            WithStructuralChanges().
            WithoutBurst().
            WithNone<DockedTag>().
            ForEach((Entity e, TrainComponent train, in DestinationComponent destination, in LocalToWorld localToWorld) =>
            {
                
                bool destinationIsPlatformStart = false;
                if (lineWaypointComponents.HasComponent(destination.Target))
                {
                    destinationIsPlatformStart = lineWaypointComponents[destination.Target].Type == RailMarkerType.PLATFORM_START;
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
                            var wagon = train.Wagons.Ptr[i];

                            // Dock wagons
                            EntityManager.AddComponent<DockedTag>(wagon);
                        }
                    }
                }
            }).Run();

        return inputDeps;
    }
}