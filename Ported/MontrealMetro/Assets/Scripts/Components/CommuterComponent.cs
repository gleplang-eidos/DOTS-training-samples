using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct CommuterComponent : IComponentData
{
    public float3 targetPosition;
    public bool isAtTargetPosition;
}