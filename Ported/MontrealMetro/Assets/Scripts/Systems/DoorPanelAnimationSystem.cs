using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(DockSystem))]
public class DoorPanelAnimationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var dockedWagons = GetComponentDataFromEntity<DockedTag>(true);

        var jobHandle = Entities.
            WithReadOnly(dockedWagons).
            ForEach((Entity entity, ref Translation translation, ref DoorPanelComponent doorPanelComponent) =>
        {
            switch (doorPanelComponent.DoorState)
            {
                case DoorState.Closed:
                    {
                        if (dockedWagons.HasComponent(entity))
                        {
                            doorPanelComponent.DoorState = DoorState.Opening;
                        }
                    }
                    break;
                case DoorState.Closing:
                    {
                        float displacement = doorPanelComponent.DoorDirection == DoorDirection.Left ? -0.3f : 0.3f;
                        translation.Value = new float3(translation.Value.x - displacement, translation.Value.y, translation.Value.z);
                        doorPanelComponent.DoorState = DoorState.Closed;
                    }          
                    break;
                case DoorState.Open:
                    break;
                case DoorState.Opening:
                    {
                        float displacement = doorPanelComponent.DoorDirection == DoorDirection.Left ? -0.3f : 0.3f;
                        translation.Value = new float3(translation.Value.x + displacement, translation.Value.y, translation.Value.z);
                        doorPanelComponent.DoorState = DoorState.Open;
                    }                   
                    break;

            }

        }).Schedule(inputDeps);

        return jobHandle;
    }
}
