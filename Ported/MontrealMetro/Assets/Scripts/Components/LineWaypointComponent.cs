using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct LineWaypointComponent : IComponentData
{
    public LineColor LineID;
    public int Index;
    public RailMarkerType Type;
}
