using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PlatformComponent : IComponentData
{
    public Entity DockedTrain;
    public float DockedTime;
}
