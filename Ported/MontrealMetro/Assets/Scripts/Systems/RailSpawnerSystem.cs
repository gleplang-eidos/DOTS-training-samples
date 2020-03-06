using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class RailSpawnerSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithStructuralChanges().ForEach(
            (Entity e,
            ref Translation translation,
            ref Rotation rotation,
            ref RailSpawnComponent railSpawnComponent,
            ref RailComponent railComponent
            ) =>
        {
            var rail = EntityManager.Instantiate(railSpawnComponent.RailPrefab);

            // Instanciate the platform if necessary
            if (railComponent.Type == RailMarkerType.PLATFORM_END)
            {
                railComponent.Platform = EntityManager.Instantiate(railSpawnComponent.PlatformPrefab);
                EntityManager.SetComponentData(railComponent.Platform, new Translation { Value = translation.Value });

                var platformRotation = quaternion.AxisAngle(new float3(0f, 1f, 0f), math.radians(90f));
                EntityManager.SetComponentData(railComponent.Platform, new Rotation { Value = math.mul(rotation.Value,platformRotation) });
            }
            EntityManager.SetComponentData(rail, new Translation { Value = translation.Value });
            EntityManager.SetComponentData(rail, new Rotation { Value = rotation.Value });

            EntityManager.AddComponentData(rail, railComponent);

            switch(railSpawnComponent.LineColor)
            {
                case LineColor.Blue:
                    EntityManager.AddComponent(rail, typeof(BlueLineTag));
                    break;
                case LineColor.Green:
                    EntityManager.AddComponent(rail, typeof(GreenLineTag));
                    break;
                case LineColor.Orange:
                    EntityManager.AddComponent(rail, typeof(OrangeLineTag));
                    break;
                case LineColor.Yellow:
                    EntityManager.AddComponent(rail, typeof(YellowLineTag));
                    break;
            }

            EntityManager.DestroyEntity(e);

        }).Run();

        return default;
    }
}
