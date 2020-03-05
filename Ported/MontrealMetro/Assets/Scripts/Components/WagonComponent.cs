using System;
using Unity.Collections;
using Unity.Entities;

public struct WagonComponent : IComponentData
{
    public FixedList128<Entity> Seats;
}
