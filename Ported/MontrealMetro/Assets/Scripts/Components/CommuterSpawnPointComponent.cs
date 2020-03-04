using System;
using Unity.Entities;

[Serializable]
public struct CommuterSpawnPointComponent : IComponentData
{
    public Entity CommuterPrefab;
} 


