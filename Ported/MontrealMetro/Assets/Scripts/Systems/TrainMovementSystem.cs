using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class TrainMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.
            WithoutBurst().
            WithNone<DockedTag>().
            ForEach((Entity entity, ref TrainComponent train, ref Translation translation, ref Rotation rotation) =>
        {
            translation.Value.z += train.Speed;
        }).Run();

        return inputDeps;
    }
}
