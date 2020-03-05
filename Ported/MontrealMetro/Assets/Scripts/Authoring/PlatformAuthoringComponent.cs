using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class PlatformAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    float DockedTime;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlatformComponent { DockedTime = DockedTime });
        dstManager.AddBuffer<QueueBufferElementData>(entity);
    }
}
