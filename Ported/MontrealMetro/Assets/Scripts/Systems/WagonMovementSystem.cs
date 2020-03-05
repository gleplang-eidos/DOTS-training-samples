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

        var blueEntities = m_BlueLineQuery.ToEntityArray(Allocator.TempJob);
        var greenEntities = m_GreenLineQuery.ToEntityArray(Allocator.TempJob);
        var orangeEntities = m_OrangeLineQuery.ToEntityArray(Allocator.TempJob);
        var yellowEntities = m_YellowLineQuery.ToEntityArray(Allocator.TempJob);

        Entities
            .WithReadOnly(localToWorlds)
            .WithoutBurst()
            .WithNone<DockedTag>()
            .ForEach((Entity entity, TrainComponent train, ref Translation translation,
            ref Rotation rotation, ref DestinationComponent destination) =>
        {
            if (destination.Target != Entity.Null)
            {
                Entity destinationEntity;

                float3 destinationPosition = new float3();

                if (EntityManager.HasComponent(destination.Target, typeof(BlueLineTag)))
                {
                    destinationEntity = destination.Target;
                    destinationPosition = EntityManager.GetComponentData<LocalToWorld>(destinationEntity).Position;

                    //destinationPosition = GetPositionBy(m_BlueLineQuery, destinationEntity, localToWorlds);

                    //destinationPosition = localToWorlds[destinationEntity].Position;
                }
                else if (EntityManager.HasComponent(destination.Target, typeof(GreenLineTag)))
                {
                    destinationEntity = destination.Target;
                    destinationPosition = EntityManager.GetComponentData<LocalToWorld>(destinationEntity).Position;
                    //destinationPosition = localToWorlds[destinationEntity].Position;
                }
                else if (EntityManager.HasComponent(destination.Target, typeof(OrangeLineTag)))
                {
                    destinationEntity = destination.Target;
                    destinationPosition = localToWorlds[destinationEntity].Position;
                }
                else if (EntityManager.HasComponent(destination.Target, typeof(YellowLineTag)))
                {
                    destinationEntity = destination.Target;
                    destinationPosition = localToWorlds[destinationEntity].Position;
                }

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

        return inputDeps;
    }


}
