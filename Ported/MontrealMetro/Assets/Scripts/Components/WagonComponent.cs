using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct WagonComponent : IComponentData
{
    public int WagonIndex;
    public FixedList128<Entity> Seats;
    public Entity LeftEntryEntity;
    public Entity RightEntryEntity;
}
