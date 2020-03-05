using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct WagonComponent : IComponentData
{
    public FixedList128<Entity> Seats;
    public float3 LeftEntryPosition;
    public float3 RightEntryPosition;
}
