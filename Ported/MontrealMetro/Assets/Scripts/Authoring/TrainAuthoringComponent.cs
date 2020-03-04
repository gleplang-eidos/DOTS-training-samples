using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class TrainAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Get all the children
        var children = gameObject.GetComponentsInChildren<WagonMonoTag>();

        // Create the shared component
        var train = new TrainComponent { Wagons = new UnsafeList<Entity> (children.Length, Unity.Collections.Allocator.Persistent) };

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

        foreach (var childTransform in children)
        {
            dstManager.AddSharedComponentData(conversionSystem.GetPrimaryEntity(childTransform.gameObject), train);
        }
    }
}
