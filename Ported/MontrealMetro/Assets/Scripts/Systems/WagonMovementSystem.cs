using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WagonMovementSystem : JobComponentSystem
{
    EntityQuery lineQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        lineQuery = GetEntityQuery(typeof(LineComponent));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var localToWorlds = GetComponentDataFromEntity<LocalToWorld>(true);
        var localToParents = GetComponentDataFromEntity<LocalToParent>(true);
        var lineWaypoints = GetComponentDataFromEntity<LineWaypointComponent>(true);
        var lines = lineQuery.ToComponentDataArray<LineComponent>(Unity.Collections.Allocator.TempJob);

        Entities
            .WithReadOnly(lineWaypoints)
            .WithReadOnly(localToWorlds)
            .WithReadOnly(localToParents)
            .WithoutBurst()
            .WithNone<DockedTag>()
            .ForEach((Entity entity, TrainComponent train, ref Translation translation,
            ref Rotation rotation,  ref DestinationComponent destination) =>
        {
            var destinationPosition = localToWorlds[destination.Target].Position;
            var direction = math.normalize(destinationPosition - localToWorlds[entity].Position);
            translation.Value += direction * train.Speed;
            rotation.Value = quaternion.LookRotation(direction, new float3(0,1,0));

            if (math.lengthsq(localToWorlds[entity].Position - destinationPosition) < 1f)
            {
                var oldWaypoint = lineWaypoints[destination.Target];
                // Find to which line the old way point belonged
                LineComponent line = lines[0];
                for (int i = 0; i < lines.Length; i++)
                {
                    if (oldWaypoint.LineID == lines[i].LineID)
                    {
                        line = lines[i];
                    }
                }

                unsafe
                {
                    var newWaypointIndex = oldWaypoint.Index == (line.Waypoints.Length - 1) ? 0 : oldWaypoint.Index + 1;
                    destination.Target = line.Waypoints.Ptr[newWaypointIndex];
                }

            }
        }
        ).Run();

        lines.Dispose();

        return inputDeps;
    }
}
