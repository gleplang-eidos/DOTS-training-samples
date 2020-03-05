using System;
using Unity.Entities;

[Serializable]
public struct RailSpawnComponent : IComponentData
{
    public Entity RailPrefab;
    public int RailID;
    public LineColor LineColor;
} 


