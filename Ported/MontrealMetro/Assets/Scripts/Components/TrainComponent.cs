﻿using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TrainComponent : ISharedComponentData
{
    public UnsafeList<Entity> Wagons;
    public FixedList512<Entity> Doors;

    public float Speed;
}
