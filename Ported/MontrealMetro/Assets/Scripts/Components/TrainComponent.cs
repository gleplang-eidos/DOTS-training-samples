﻿using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct TrainComponent : ISharedComponentData
{
    public FixedList128<Entity> Wagons;
    public FixedList512<Entity> Doors;
    public Entity Platform;
    public float Speed;
    public LineColor LineColor;
    public float WagonOffset;
    public Entity HeadWagon;
    public int StationIndex;
}
