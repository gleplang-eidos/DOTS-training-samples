using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

[UpdateAfter(typeof(WagonBoardingSystem))]
public class CommuterNavigationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var deltaTime = Time.DeltaTime;
        Entities.WithoutBurst().ForEach((Entity e, ref CommuterComponent commuter, ref Translation translation, ref Rotation rotation) =>
        {
            if(!commuter.isAtTargetPosition)
            {
                float3 direction = new float3(commuter.targetPosition.x, 0, commuter.targetPosition.z)
            - new float3(translation.Value.x, 0, translation.Value.z);

                // already there
                if (math.length(direction) < 0.1f)
                {
                    commuter.isAtTargetPosition = true;
                    return;
                }

                direction = math.normalize(direction);
                translation.Value += direction * commuter.speed * deltaTime;
            }
        }).Run();

        return default;
    }
}
