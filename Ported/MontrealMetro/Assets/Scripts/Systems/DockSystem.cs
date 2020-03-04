using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(TrainMovementSystem))]
public class DockSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.
            WithStructuralChanges().
            WithoutBurst().
            WithNone<DockedTag>().
            ForEach((TrainComponent train, in LocalToWorld localToWorld) =>
        {
            // If close enough, stop every wagon by adding the docked tag.
            if (math.abs(math.length(train.Destination - localToWorld.Position)) <= 1)
            {
                for (var i = 0; i < train.Wagons.Length; ++i)
                {
                    unsafe
                    {
                        EntityManager.AddComponent<DockedTag>(train.Wagons.Ptr[i]);
                    }
                }
            }
        }).Run();

        return inputDeps;
    }
}
