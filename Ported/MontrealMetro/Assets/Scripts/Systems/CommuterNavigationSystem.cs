using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public class CommuterNavigationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var deltaTime = Time.DeltaTime;
        Entities.ForEach((Entity e, CommuterComponent commuter, ref Translation translation, ref Rotation rotation) =>
        {
            float3 direction = new float3(commuter.targetPosition.x, 0, commuter.targetPosition.z)
            - new float3(translation.Value.x, 0, translation.Value.z);

            // already there
            if (math.length(direction) < 0.1f)
            {
                return;
            }

            direction = math.normalize(direction);
            translation.Value += direction * commuter.speed * deltaTime;

        }).Run();

        return default;
    }
}
