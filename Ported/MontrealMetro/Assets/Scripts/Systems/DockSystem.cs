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
        Entities.
            WithReadOnly(localToWorlds).
            WithReadOnly(lineWaypointComponents).
            WithStructuralChanges().
            WithoutBurst().
            WithNone<DockedTag>().
            ForEach((Entity e, TrainComponent train, ref DestinationComponent destination, in LocalToWorld localToWorld) =>
            {
                bool destinationIsPlatformStart = false;
                if (lineWaypointComponents.HasComponent(destination.Target))
                {
                    destinationIsPlatformStart = lineWaypointComponents[destination.Target].Type == RailMarkerType.PLATFORM_START;
                }

                // If close enough to a dock entry, stop every wagon by adding the docked tag.
                if (destinationIsPlatformStart && math.abs(math.length(localToWorlds[destination.Target].Position - localToWorld.Position)) <= 1)
                {
                    for (var i = 0; i < train.Wagons.Length; ++i)
                    {
                        unsafe
                        {
                            EntityManager.AddComponent<DockedTag>(train.Wagons.Ptr[i]);
                        }
                    }
                }
            }).Run();

        return inputDeps;
    }
}