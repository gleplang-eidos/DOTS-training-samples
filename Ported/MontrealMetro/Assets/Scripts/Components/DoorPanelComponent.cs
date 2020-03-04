using System;
using Unity.Entities;

[Serializable]
[GenerateAuthoringComponent]
public struct DoorPanelComponent : IComponentData
{ 
    public DoorDirection DoorDirection;
    public DoorState DoorState;
}
