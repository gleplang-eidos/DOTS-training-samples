using System;
using Unity.Entities;

[Serializable]
public struct EntityBufferElementData : IBufferElementData
{
    public Entity entity;
}
