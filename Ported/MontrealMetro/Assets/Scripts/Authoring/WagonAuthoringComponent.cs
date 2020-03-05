using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class WagonAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField]
    List<GameObject> Seats = new List<GameObject>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        FixedList128<Entity> fixedList = new FixedList128<Entity>();
               
        for (int i = 0; i < Seats.Count; i++)
        {
            Entity seat = conversionSystem.GetPrimaryEntity(Seats[i]);
            fixedList.Add(seat);
        }

        dstManager.AddComponentData(entity, new WagonComponent { Seats = fixedList });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        for (int i = 0; i < Seats.Count; i++)
        {
            referencedPrefabs.Add(Seats[i]);
        }
    }
}
