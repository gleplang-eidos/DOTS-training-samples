using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class PlatformAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField]
    float DockedTime;
    [SerializeField]
    List<GameObject> Queues = new List<GameObject>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PlatformComponent { DockedTime = DockedTime });
        var buffer = dstManager.AddBuffer<QueueBufferElementData>(entity);

        for (int i = 0; i < Queues.Count; i++)
        {
            Entity platform = conversionSystem.GetPrimaryEntity(Queues[i]);
            buffer.Add(new QueueBufferElementData() { entity = platform });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        for (int i = 0; i < Queues.Count; i++)
        {
            referencedPrefabs.Add(Queues[i]);
        }
    }
}
