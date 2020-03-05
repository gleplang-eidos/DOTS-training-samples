using System;
using Unity.Entities;

[Serializable]
public struct QueueBufferElementData : IBufferElementData
{
    public Entity entity;
}
