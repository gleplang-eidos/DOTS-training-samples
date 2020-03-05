using UnityEngine;
using System.Collections;
using Unity.Entities;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class CommuterSpawnPointAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject CommuterPrefab;
    public GameObject Platform;
    public int nbCommutersToSpawn;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Entity commuterPrefab = conversionSystem.GetPrimaryEntity(CommuterPrefab);
        Entity platform = conversionSystem.GetPrimaryEntity(Platform);
        dstManager.AddComponentData(entity, new CommuterSpawnPointComponent { CommuterPrefab = commuterPrefab, Platform = platform, NbCommutersToSpawn = nbCommutersToSpawn });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(CommuterPrefab);
        referencedPrefabs.Add(Platform);
    }
}
