using UnityEngine;
using System.Collections;
using Unity.Entities;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class QueueAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new QueueComponent { PositioningOffset = 0.2f });
        dstManager.AddBuffer<EntityBufferElementData>(entity);
    }
}
