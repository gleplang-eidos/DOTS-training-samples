using UnityEngine;
using UnityEditor;

public enum RailMarkerType
{
	PLATFORM_START,
	PLATFORM_END,
	ROUTE
}

[ExecuteInEditMode]
public class RailMarker : MonoBehaviour
{

	public int metroLineID;
	public int pointIndex;
	public RailMarkerType railMarkerType;
	public LineColor LineColor;

	private void Awake()
	{
		pointIndex = GetSiblingIndex(transform, transform.parent);
		name = metroLineID + "_" + pointIndex;
	}
	public void OnDrawGizmos()
	{
		Gizmos.color = GUI.color = (railMarkerType != RailMarkerType.PLATFORM_START) ?  GetLineColor(LineColor) : Color.white;
		
		// Draw marker X
		float xSize = 0.5f;
		Gizmos.DrawLine(transform.position + new Vector3(-xSize, 0f, -xSize), transform.position + new Vector3(xSize, 0f, xSize));
		Gizmos.DrawLine(transform.position + new Vector3(xSize, 0f, -xSize), transform.position + new Vector3(-xSize, 0f, xSize));
		
		// connect to next in line (if found)
		if (pointIndex != transform.parent.childCount-1)
		{
			Gizmos.DrawLine(transform.position, transform.parent.GetChild(pointIndex+1).position);
		}
		
		Handles.Label(transform.position + new Vector3(0f,1f,0f), metroLineID+"_"+pointIndex + ((railMarkerType == RailMarkerType.PLATFORM_START) ? " **" : ""));
	}
	
	int GetSiblingIndex(Transform child, Transform parent)
	{
		int result = 0;
		for (int i = 0; i < parent.childCount; ++i)
		{
			if (child == parent.GetChild(i))
				
				result = i;
		}
		return result;
	}

    Color GetLineColor(LineColor lineColor)
    {
        switch(lineColor)
        {
			case LineColor.Blue:
				return Color.blue;
			case LineColor.Green:
				return Color.green;
			case LineColor.Orange:
				return new Color(1, 0.5f, 0);
			case LineColor.Yellow:
				return Color.yellow;
			default:
				return Color.magenta;
		}
    }
}
