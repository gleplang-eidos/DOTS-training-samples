using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Transforms;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class TrainAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    GameObject Destination = null;

    [SerializeField]
    float Speed = 0.002f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Get all the children
        var children = gameObject.GetComponentsInChildren<WagonMonoTag>();

        // Create the shared component
        var train = new TrainComponent { Wagons = new UnsafeList<Entity> (children.Length, Unity.Collections.Allocator.Persistent), Speed = Speed };

        // Fill the list of wagon as well as assigning the shared component to every wagon.
        //foreach (var childTransform in children)
        //{
        //    var wagonEntity = conversionSystem.GetPrimaryEntity(childTransform.gameObject);
        //    train.Wagons.Add(wagonEntity);
        //    dstManager.AddSharedComponentData(wagonEntity, train);
        //}

        foreach (var childTransform in children)
        {
            train.Wagons.Add(conversionSystem.GetPrimaryEntity(childTransform.gameObject));
        }

        var destinationEntity = conversionSystem.GetPrimaryEntity(Destination);

        foreach (var childTransform in children)
        {
            var wagonEntity = conversionSystem.GetPrimaryEntity(childTransform.gameObject);
            dstManager.AddSharedComponentData(wagonEntity, train);
            dstManager.AddComponentData(wagonEntity, new DestinationComponent { Target = destinationEntity });
            dstManager.RemoveComponent<LocalToParent>(wagonEntity);
            dstManager.RemoveComponent<Parent>(wagonEntity);
            dstManager.SetComponentData(wagonEntity, new Translation { Value = childTransform.transform.position });
            dstManager.SetComponentData(wagonEntity, new Rotation { Value = childTransform.transform.rotation });
        }

        dstManager.DestroyEntity(entity);
    }
}
