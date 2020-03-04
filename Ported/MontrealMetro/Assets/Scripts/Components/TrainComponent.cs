using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[Serializable]
public struct TrainComponent : ISharedComponentData
{
    public UnsafeList<Entity> Wagons;
    public float Speed;
    public float3 Destination;
}
