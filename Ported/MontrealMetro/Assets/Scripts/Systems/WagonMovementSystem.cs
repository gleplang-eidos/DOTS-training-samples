using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class WagonMovementSystem : JobComponentSystem
{
    EntityQuery m_BlueLineQuery;
    EntityQuery m_GreenLineQuery;
    EntityQuery m_OrangeLineQuery;
    EntityQuery m_YellowLineQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_BlueLineQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<RailComponent>(), ComponentType.ReadOnly<BlueLineTag>()
            }
            ,
            None = new[]
            {
                ComponentType.ReadOnly<RailSpawnComponent>()
            }
        });

        m_GreenLineQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
           {
                ComponentType.ReadOnly<RailComponent>(), ComponentType.ReadOnly<GreenLineTag>()
            }
           ,
            None = new[]
           {
                ComponentType.ReadOnly<RailSpawnComponent>()
            }
        });

        m_OrangeLineQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
           {
                ComponentType.ReadOnly<RailComponent>(), ComponentType.ReadOnly<OrangeLineTag>()
            }
           ,
            None = new[]
           {
                ComponentType.ReadOnly<RailSpawnComponent>()
            }
        });

        m_YellowLineQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
           {
                ComponentType.ReadOnly<RailComponent>(), ComponentType.ReadOnly<YellowLineTag>()
            }
           ,
            None = new[]
           {
                ComponentType.ReadOnly<RailSpawnComponent>()
            }
        });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if(m_GreenLineQuery.CalculateEntityCount() == 0)
        {
            return default;
        }

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
                    FindNextDestination(entity, ref destination, localToWorlds, train, ref translation, ref rotation, m_BlueLineQuery);
                }
                else if (EntityManager.HasComponent(destination.Target, typeof(GreenLineTag)))
                {
                    FindNextDestination(entity, ref destination, localToWorlds, train, ref translation, ref rotation, m_GreenLineQuery);
                }
                else if (EntityManager.HasComponent(destination.Target, typeof(OrangeLineTag)))
                {
                    FindNextDestination(entity, ref destination, localToWorlds, train, ref translation, ref rotation, m_OrangeLineQuery);
                }
                else if (EntityManager.HasComponent(destination.Target, typeof(YellowLineTag)))
                {
                    FindNextDestination(entity, ref destination, localToWorlds, train, ref translation, ref rotation, m_YellowLineQuery);
                }
            }
            else
            {
                destination.Target = GetFirstStation(train.LineColor);

                var firstPosition = EntityManager.GetComponentData<Translation>(destination.Target);

                firstPosition.Value.z -= train.WagonOffset * EntityManager.GetComponentData<WagonComponent>(entity).WagonIndex;

                var direction = math.normalize(firstPosition.Value - localToWorlds[entity].Position);

                rotation.Value = quaternion.LookRotation(direction, new float3(0, 1, 0));

                translation.Value = firstPosition.Value;

                EntityManager.SetComponentData(entity, translation);

                EntityManager.SetComponentData(entity, rotation);

                FindNextDestination(entity, ref destination, localToWorlds, train, ref translation, ref rotation, m_GreenLineQuery);
            }
        }).Run();

        return inputDeps;
    }

    Entity GetFirstStation(LineColor lineColor)
    {
        EntityQuery query = m_BlueLineQuery;

        switch(lineColor)
        {
            case LineColor.Blue:
                query = m_BlueLineQuery;
                break;
            case LineColor.Green:
                query = m_GreenLineQuery;
                break;
            case LineColor.Orange:
                query = m_OrangeLineQuery;
                break;
            case LineColor.Yellow:
                query = m_YellowLineQuery;
                break;
        }

        var railEntities = query.ToEntityArray(Allocator.TempJob);

        var firstStation = railEntities[0];

        for (int i = 0, length = railEntities.Length; i < length; ++i)
        {
            var railComponent = EntityManager.GetComponentData<RailComponent>(railEntities[i]);
            if(railComponent.Type == RailMarkerType.PLATFORM_START)
            {
                firstStation = railEntities[i];
                break;
            }
        }

        railEntities.Dispose();

        return firstStation;
    }

    void FindNextDestination(Entity entity,
        ref DestinationComponent destination,
        ComponentDataFromEntity<LocalToWorld> localToWorlds,
        TrainComponent train,
        ref Translation translation,
        ref Rotation rotation,
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

            var matchIndex = destRailComponent.RailID == railEntities.Length - 1 ? 0 : destRailComponent.RailID + 1;

            for (int i = 0, length = railEntities.Length; i < length; ++i)
            {
                var railComponent = EntityManager.GetComponentData<RailComponent>(railEntities[i]);

                if (railComponent.RailID == matchIndex)
                {
                    destination.Target = railEntities[i];

                    EntityManager.SetComponentData(entity, destination);

                    break;
                }
            }

            railEntities.Dispose();
        }
    }
}
