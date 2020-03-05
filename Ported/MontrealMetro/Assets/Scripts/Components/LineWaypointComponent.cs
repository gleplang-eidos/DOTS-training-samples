using Unity.Entities;

public struct LineWaypointComponent : IComponentData
{
    public LineColor LineID;
    public int Index;
    public RailMarkerType Type;
}
