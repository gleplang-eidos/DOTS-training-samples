using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct QueueComponent : IComponentData
{
    public float3 Position;
    public float PositioningOffset;
    
} 


