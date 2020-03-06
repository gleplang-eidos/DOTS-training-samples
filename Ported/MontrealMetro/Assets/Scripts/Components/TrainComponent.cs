using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

[Serializable]
public struct TrainComponent : ISharedComponentData
{
    public FixedList128<Entity> Wagons;
    public FixedList512<Entity> Doors;
    public Entity Platform;
    public float Speed;
    public LineColor LineColor;
}
