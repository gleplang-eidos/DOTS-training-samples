using System;
using Unity.Entities;

[Serializable]
public struct CommuterSpawnPointComponent : IComponentData
{
    public Entity CommuterPrefab;
    public Entity Platform;
    public int NbCommutersToSpawn;
} 


