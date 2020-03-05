using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

[Serializable]
public struct TrainComponent : ISharedComponentData
{
    public UnsafeList<Entity> Wagons;
    public FixedList512<Entity> Doors;

    public float Speed;
    public LineColor LineColor;
}
