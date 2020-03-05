using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

[Serializable]
public struct TrainComponent : ISharedComponentData
{
    public UnsafeList<Entity> Wagons;
    public float Speed;
    public LineColor LineColor;
}
