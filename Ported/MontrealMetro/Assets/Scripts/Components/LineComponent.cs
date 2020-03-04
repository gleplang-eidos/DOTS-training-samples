using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Collections.LowLevel.Unsafe;

public struct LineComponent : IComponentData
{
    public LineColor LineID;
    public UnsafeList<Entity> Waypoints; 
}
