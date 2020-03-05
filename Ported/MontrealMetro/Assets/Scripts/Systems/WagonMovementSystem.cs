using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

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

        Entities
            .WithReadOnly(localToWorlds)
            .WithoutBurst()
            .WithNone<DockedTag>()
            .ForEach((Entity entity, TrainComponent train, ref Translation translation,
            ref Rotation rotation, ref DestinationComponent destination) =>
        {
            if (destination.Target != Entity.Null)
            {
                if (EntityManager.HasComponent(destination.Target, typeof(BlueLineTag)))
                {
                    Continue(entity, destination, localToWorlds, train, translation, rotation, m_BlueLineQuery);
                }
                else if (EntityManager.HasComponent(destination.Target, typeof(GreenLineTag)))
                {
                    Continue(entity, destination, localToWorlds, train, translation, rotation, m_GreenLineQuery);
                }
                else if (EntityManager.HasComponent(destination.Target, typeof(OrangeLineTag)))
                {
                    Continue(entity, destination, localToWorlds, train, translation, rotation, m_OrangeLineQuery);
                }
                else if (EntityManager.HasComponent(destination.Target, typeof(YellowLineTag)))
                {
                    Continue(entity, destination, localToWorlds, train, translation, rotation, m_YellowLineQuery);
                }
            }
        }).Run();

        return inputDeps;
    }

    void Continue(Entity entity,
        DestinationComponent destination,
        ComponentDataFromEntity<LocalToWorld> localToWorlds,
        TrainComponent train,
        Translation translation,
        Rotation rotation,
        EntityQuery query)
    {
        float3 destinationPosition = EntityManager.GetComponentData<LocalToWorld>(destination.Target).Position;

        var direction = math.normalize(destinationPosition - localToWorlds[entity].Position);
        translation.Value += direction * train.Speed;
        rotation.Value = quaternion.LookRotation(direction, new float3(0, 1, 0));

        if (math.lengthsq(localToWorlds[entity].Position - destinationPosition) < 1f)
        {
            var railEntities = query.ToEntityArray(Allocator.TempJob);

            var destRailComponent = EntityManager.GetComponentData<RailComponent>(destination.Target);

            var matchIndex = 0;

            if(destRailComponent.RailID == railEntities.Length - 1)
            {
                matchIndex = 0;
            }
            else
            {
                matchIndex = destRailComponent.RailID + 1;
            }

            for (int i = 0, length = railEntities.Length; i < length; ++i)
            {
                var railComponent = EntityManager.GetComponentData<RailComponent>(railEntities[i]);
                if (railComponent.RailID == matchIndex)
                {
                    destination.Target = railEntities[i];
                    break;
                }
            }

            railEntities.Dispose();
        }
    }
}
