using UnityEngine;
using Unity.Entities;
using System.Collections;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

public class LineAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    LineColor m_LineColor;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var railMarkers = GetComponentsInChildren<RailMarker>();
        UnsafeList<Entity> waypointList = new UnsafeList<Entity>(railMarkers.Length, Unity.Collections.Allocator.Persistent);

        for (int i = 0; i < railMarkers.Length; i++)
        {
            var waypointEntity = conversionSystem.GetPrimaryEntity(railMarkers[i]);
            dstManager.AddComponentData(waypointEntity, new LineWaypointComponent() { LineID = m_LineColor, Index = i });
            waypointList.Add(waypointEntity);
        }

        dstManager.AddComponentData(entity, new LineComponent { LineID = m_LineColor, Waypoints = waypointList });
    }
}
