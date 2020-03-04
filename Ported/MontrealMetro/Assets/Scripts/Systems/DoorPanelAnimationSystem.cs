using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class DoorPanelAnimationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var jobHandle = Entities.ForEach((ref Translation translation, ref DoorPanelComponent doorPanelComponent) =>
        {
            switch (doorPanelComponent.DoorState)
            {
                case DoorState.Closed:
                    break;
                case DoorState.Closing:
                    {
                        float displacement = doorPanelComponent.DoorDirection == DoorDirection.Left ? 0.3f : -0.3f;
                        translation.Value = new float3(translation.Value.x + displacement, 0, translation.Value.z);
                        doorPanelComponent.DoorState = DoorState.Closed;
                    }          
                    break;
                case DoorState.Open:
                    break;
                case DoorState.Opening:
                    {
                        float displacement = doorPanelComponent.DoorDirection == DoorDirection.Left ? -0.3f : 0.3f;
                        translation.Value = new float3(translation.Value.x + displacement, 0, translation.Value.z);
                        doorPanelComponent.DoorState = DoorState.Open;
                    }                   
                    break;

            }

        }).Schedule(inputDeps);

        return jobHandle;
    }
}
