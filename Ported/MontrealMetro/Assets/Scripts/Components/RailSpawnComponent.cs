﻿using System;
using Unity.Entities;

[Serializable]
public struct RailSpawnComponent : IComponentData
{
    public Entity RailPrefab;
    public Entity PlatformPrefab;

    public LineColor LineColor;
} 


