using System;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct SeatComponent : IComponentData
{
    public Entity Passenger;
}
