using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class WagonMovementSystem : JobComponentSystem
{
    EntityQuery m_BlueLineQuery;
    EntityQuery m_GreenLineQuery;
    EntityQuery m_OrangeLineQuery;
    EntityQuery m_YellowLineQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_BlueLineQuery = GetEntityQuery(typeof(RailComponent), typeof(BlueLineTag));
        m_GreenLineQuery = GetEntityQuery(typeof(RailComponent), typeof(GreenLineTag));
        m_OrangeLineQuery = GetEntityQuery(typeof(RailComponent), typeof(OrangeLineTag));
        m_YellowLineQuery = GetEntityQuery(typeof(RailComponent), typeof(YellowLineTag));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var localToWorlds = GetComponentDataFromEntity<LocalToWorld>(true);
        //var localToParents = GetComponentDataFromEntity<LocalToParent>(true);
        /*var lineWaypoints = GetComponentDataFromEntity<LineWaypointComponent>(true);
        var lines = lineQuery.ToComponentDataArray<LineComponent>(Unity.Collections.Allocator.TempJob);*/

        //var lineWaypoints = GetComponentDataFromEntity<LineWaypointComponent>(true);
        //var lines = m_LineQuery.ToComponentDataArray<LineComponent>(Unity.Collections.Allocator.TempJob);

        var blueEntities = m_BlueLineQuery.ToEntityArray(Allocator.TempJob);
        var greenEntities = m_GreenLineQuery.ToEntityArray(Allocator.TempJob);
        var orangeEntities = m_OrangeLineQuery.ToEntityArray(Allocator.TempJob);
        var yellowEntities = m_YellowLineQuery.ToEntityArray(Allocator.TempJob);

        Entities
            //.WithReadOnly(lineWaypoints)
            .WithReadOnly(localToWorlds)
            //.WithReadOnly(localToParents)
            .WithoutBurst()
            .WithNone<DockedTag>()
            .ForEach((Entity entity, TrainComponent train, ref Translation translation,
            ref Rotation rotation, ref DestinationComponent destination) =>
        {
            if (destination.Target != Entity.Null)
            {
                if(EntityManager.HasComponent(destination.Target, typeof(BlueLineTag)))
                {

                }

                var destinationPosition = localToWorlds[destination.Target].Position;
                var direction = math.normalize(destinationPosition - localToWorlds[entity].Position);
                translation.Value += direction * train.Speed;
                rotation.Value = quaternion.LookRotation(direction, new float3(0, 1, 0));

                if (math.lengthsq(localToWorlds[entity].Position - destinationPosition) < 1f)
                {
                    




                    /*var oldWaypoint = lineWaypoints[destination.Target];
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
                    }*/

                    
                }
            }
        }).Run();

        blueEntities.Dispose();
        greenEntities.Dispose();
        orangeEntities.Dispose();
        yellowEntities.Dispose();

        //lines.Dispose();

        return inputDeps;
    }
}
