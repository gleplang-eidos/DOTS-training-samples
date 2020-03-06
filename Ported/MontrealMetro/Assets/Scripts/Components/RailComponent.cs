using Unity.Entities;

public struct RailComponent : IComponentData
{
    public int RailID;
    public RailMarkerType Type;
    public Entity Platform;
}
