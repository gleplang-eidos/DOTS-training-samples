using Unity.Entities;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class TrainAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField]
    [HideInInspector]
    GameObject Destination = null;

    [SerializeField]
    LineColor LineColor;

    [SerializeField]
    float Speed = 0.002f;

    [SerializeField]
    List<GameObject> Doors = new List<GameObject>();

    [SerializeField]
    GameObject platform;

    [SerializeField]
    float WagonOffset = 5.25f;

    [SerializeField]
    int StationIndex = 0;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Get all the children
        var children = gameObject.GetComponentsInChildren<WagonMonoTag>();        var railMarker = Destination != null ? Destination.GetComponent<RailMarker>() : null;

        var doors = new FixedList512<Entity>();
        foreach (var door in Doors)
        {
            doors.Add(conversionSystem.GetPrimaryEntity(door));
        }

        //var platformEntity = conversionSystem.GetPrimaryEntity(platform);
        // Create the shared component
        var train = new TrainComponent
        {
            Wagons = new FixedList128<Entity>(),
            Doors = doors,
            Speed = Speed,
            LineColor = LineColor,
            WagonOffset = WagonOffset,
            HeadWagon = conversionSystem.GetPrimaryEntity(children[children.Length - 1]),
            StationIndex = StationIndex
        };

        dstManager.AddComponent<HeadWagonTag>(conversionSystem.GetPrimaryEntity(children[children.Length - 1]));

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
            dstManager.AddComponentData(wagonEntity, new DestinationComponent { /*Target = destinationEntity*/ });
            dstManager.RemoveComponent<LocalToParent>(wagonEntity);
            dstManager.RemoveComponent<Parent>(wagonEntity);
            dstManager.SetComponentData(wagonEntity, new Translation { Value = childTransform.transform.position });
            dstManager.SetComponentData(wagonEntity, new Rotation { Value = childTransform.transform.rotation });

            if (railMarker != null)
            {
                switch (railMarker.LineColor)
                {
                    case LineColor.Blue:
                        dstManager.AddComponent(wagonEntity, typeof(BlueLineTag));
                        break;
                    case LineColor.Green:
                        dstManager.AddComponent(wagonEntity, typeof(GreenLineTag));
                        break;
                    case LineColor.Orange:
                        dstManager.AddComponent(wagonEntity, typeof(OrangeLineTag));
                        break;
                    case LineColor.Yellow:
                        dstManager.AddComponent(wagonEntity, typeof(YellowLineTag));
                        break;
                }
            }
        }

        dstManager.DestroyEntity(entity);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(platform);
    }
}