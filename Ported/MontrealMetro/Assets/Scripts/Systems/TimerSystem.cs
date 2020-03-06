using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TimerSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var deltaTime = Time.DeltaTime;
        Entities
            .WithStructuralChanges()
            .WithReadOnly(deltaTime)
            .ForEach((Entity entity, ref TimerComponent timer) =>
            {
                timer.TimerVariable -= deltaTime;
                if (timer.TimerVariable <= 0)
                {
                    EntityManager.RemoveComponent<TimerComponent>(entity);
                }
            }
        ).Run();

        return inputDeps;
    }
}
