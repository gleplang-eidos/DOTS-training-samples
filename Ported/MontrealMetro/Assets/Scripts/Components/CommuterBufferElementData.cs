using System;
using Unity.Entities;

[Serializable]
public struct CommuterBufferElementData : IBufferElementData
{
    public Entity entity;
}
