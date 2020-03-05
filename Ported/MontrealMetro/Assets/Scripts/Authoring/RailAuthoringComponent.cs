using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequiresEntityConversion]
public class RailAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField]
    GameObject m_BlueLine = null;

    [SerializeField]
    GameObject m_GreenLine = null;

    [SerializeField]
    GameObject m_OrangeLine = null;

    [SerializeField]
    GameObject m_YellowLine = null;

    [SerializeField]
    GameObject m_BlueRailPrefab = null;

    [SerializeField]
    GameObject m_GreenRailPrefab = null;

    [SerializeField]
    GameObject m_OrangeRailPrefab = null;

    [SerializeField]
    GameObject m_YellowRailPrefab = null;

    [SerializeField]
    GameObject m_EntryRailPrefab = null;

    [SerializeField]
    GameObject m_ExitRailPrefab = null;

    public const float k_BezierPlatformOffset = 3.0f;

    public const float k_RailSpacing = 0.5f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        CreateLine(
            dstManager,
            conversionSystem,
            m_BlueLine.GetComponentsInChildren<RailMarker>(),
            LineColor.Blue);

        CreateLine(
            dstManager,
            conversionSystem,
            m_GreenLine.GetComponentsInChildren<RailMarker>(),
            LineColor.Green);

        CreateLine(
            dstManager,
            conversionSystem,
            m_OrangeLine.GetComponentsInChildren<RailMarker>(),
            LineColor.Orange);

        CreateLine(
            dstManager,
            conversionSystem,
            m_YellowLine.GetComponentsInChildren<RailMarker>(),
            LineColor.Yellow);
    }

    public void CreateLine(EntityManager dstManager, GameObjectConversionSystem conversionSystem, RailMarker[] outboundPoints, LineColor lineColor)
    {
        var bezierPath = new BezierPath();
        List<BezierPoint> points = bezierPath.points;
        int total_outboundPoints = outboundPoints.Length;

        List<RailMarkerType> bezierRailTypeList = new List<RailMarkerType>();

        // - - - - - - - - - - - - - - - - - - - - - - - -  OUTBOUND points
        for (int i = 0; i < total_outboundPoints; i++)
        {
            bezierPath.AddPoint(outboundPoints[i].transform.position);

            bezierRailTypeList.Add(outboundPoints[i].railMarkerType);
        }

        // fix the OUTBOUND handles
        for (int i = 0; i <= total_outboundPoints - 1; i++)
        {
            BezierPoint currentPoint = points[i];
            if (i == 0)
            {
                currentPoint.SetHandles(points[1].location - currentPoint.location);
            }
            else if (i == total_outboundPoints - 1)
            {
                currentPoint.SetHandles(currentPoint.location - points[i - 1].location);
            }
            else
            {
                currentPoint.SetHandles(points[i + 1].location - points[i - 1].location);
            }
        }

        bezierPath.MeasurePath();

        // - - - - - - - - - - - - - - - - - - - - - - - -  RETURN points
        float platformOffset = k_BezierPlatformOffset;
        List<BezierPoint> returnPoints = new List<BezierPoint>();
        for (int i = total_outboundPoints - 1; i >= 0; i--)
        {
            Vector3 targetLocation = bezierPath.GetPoint_PerpendicularOffset(bezierPath.points[i], platformOffset);
            bezierPath.AddPoint(targetLocation);
            returnPoints.Add(points[points.Count - 1]);

            if(outboundPoints[i].railMarkerType == RailMarkerType.PLATFORM_START)
            {
                bezierRailTypeList.Add(RailMarkerType.PLATFORM_END);
            }
            else if(outboundPoints[i].railMarkerType == RailMarkerType.PLATFORM_END)
            {
                bezierRailTypeList.Add(RailMarkerType.PLATFORM_START);
            }
            else
            {
                bezierRailTypeList.Add(RailMarkerType.ROUTE);
            }
        }

        // fix the RETURN handles
        for (int i = 0; i <= total_outboundPoints - 1; i++)
        {
            BezierPoint currentPoint = returnPoints[i];
            if (i == 0)
            {
                currentPoint.SetHandles(returnPoints[1].location - currentPoint.location);
            }
            else if (i == total_outboundPoints - 1)
            {
                currentPoint.SetHandles(currentPoint.location - returnPoints[i - 1].location);
            }
            else
            {
                currentPoint.SetHandles(returnPoints[i + 1].location - returnPoints[i - 1].location);
            }
        }

        bezierPath.MeasurePath();

        var railID = 0;

        // Now, let's lay the rail meshes
        float distance = 0f;

        List<Entity> railEntityList = new List<Entity>();

        RailMarkerType currentType = RailMarkerType.ROUTE;
        RailMarkerType previousType = RailMarkerType.ROUTE;

        while (distance < bezierPath.GetPathDistance())
        {
            float distanceAsRailFactor = Get_distanceAsRailProportion(bezierPath, distance);
            Vector3 railPosition = Get_PositionOnRail(bezierPath, distanceAsRailFactor);
            Vector3 railRotation = Get_RotationOnRail(bezierPath, distanceAsRailFactor);

            var railEntity = dstManager.CreateEntity();

            railEntityList.Add(railEntity);

            var translation = new Translation { Value = new float3(railPosition.x, railPosition.y, railPosition.z) };
            var rotation = new Rotation { Value = quaternion.LookRotation(railRotation, new float3(0, 1, 0)) };

            dstManager.AddComponentData(railEntity, translation);
            dstManager.AddComponentData(railEntity, rotation);

            GameObject prefab = GetRailPrefabPerColor(lineColor);

            currentType = bezierRailTypeList[bezierPath.GetRegionIndex(distance)];

            if (currentType == RailMarkerType.ROUTE)
            {
                prefab = GetRailPrefabPerColor(lineColor);

                previousType = currentType;
            }
            else if (currentType != previousType)
            {
                previousType = currentType;

                if(currentType == RailMarkerType.PLATFORM_START)
                {
                    prefab = m_EntryRailPrefab;
                }
                else
                {
                    prefab = m_ExitRailPrefab;
                }
            }

            var railSpawn = new RailSpawnComponent
            {
                RailPrefab = conversionSystem.GetPrimaryEntity(prefab),
                RailID = railID++,
                LineColor = lineColor
            };

            dstManager.AddComponentData(railEntity, railSpawn);

            distance += k_RailSpacing;
        }
    }

    GameObject GetRailPrefabPerColor(LineColor lineColor)
    {
        switch(lineColor)
        {
            case LineColor.Blue:
                return m_BlueRailPrefab;
            case LineColor.Green:
                return m_GreenRailPrefab;
            case LineColor.Orange:
                return m_OrangeRailPrefab;
            case LineColor.Yellow:
                return m_YellowRailPrefab;
            default:
                return null;
        }
    }

    public Vector3 Get_PositionOnRail(BezierPath bezierPath, float _pos)
    {
        return bezierPath.Get_Position(_pos);
    }

    public Vector3 Get_RotationOnRail(BezierPath bezierPath, float _pos)
    {
        return bezierPath.Get_NormalAtPosition(_pos);
    }

    public float Get_distanceAsRailProportion(BezierPath bezierPath, float _realDistance)
    {
        return _realDistance / bezierPath.GetPathDistance();
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(m_BlueRailPrefab);
        referencedPrefabs.Add(m_GreenRailPrefab);
        referencedPrefabs.Add(m_OrangeRailPrefab);
        referencedPrefabs.Add(m_YellowRailPrefab);

        referencedPrefabs.Add(m_EntryRailPrefab);
        referencedPrefabs.Add(m_ExitRailPrefab);
    }
}
