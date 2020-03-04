using UnityEngine;
using System.Collections;
using Unity.Entities;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class CommuterSpawnPointAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject CommuterPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Entity prefab = conversionSystem.GetPrimaryEntity(CommuterPrefab);
        dstManager.AddComponentData(entity, new CommuterSpawnPointComponent { CommuterPrefab = prefab });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(CommuterPrefab);
    }
}
