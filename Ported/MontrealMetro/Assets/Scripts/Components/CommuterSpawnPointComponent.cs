using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct CommuterSpawnPointComponent : IComponentData
{
    public Entity CommuterPrefab;
} 


