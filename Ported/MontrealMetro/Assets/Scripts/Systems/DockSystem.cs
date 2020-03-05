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
            WithNativeDisableParallelForRestriction(doorPanels).
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

                if (destinationIsPlatformStart)
                    Debug.Log(math.length(localToWorlds[destination.Target].Position - localToWorld.Position));

                // If close enough to a dock entry, stop every wagon by adding the docked tag.
                if (destinationIsPlatformStart && math.abs(math.length(localToWorlds[destination.Target].Position - localToWorld.Position)) <= 1)
                {
                    for (var i = 0; i < train.Wagons.Length; ++i)
                    {
                        unsafe
                        {
                            var wagon = train.Wagons.Ptr[i];

                            // Dock wagons
                            EntityManager.AddComponent<DockedTag>(wagon);

                            // Open doors
                            if (doorPanels.HasComponent(wagon))
                            {
                                //var doorPanelComponent = doorPanels[wagon];
                                //doorPanelComponent.DoorState = DoorState.Opening;
                                //doorPanels[wagon] = doorPanelComponent;
                            }

                        }
                    }
                }
            }).Run();

        return inputDeps;
    }
}