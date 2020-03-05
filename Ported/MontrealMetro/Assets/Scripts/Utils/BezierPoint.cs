using System.Collections.Generic;
using UnityEngine;

public class BezierPoint
{
    public static float s_BezierHandleReach = 0.1f;

    public int index;
    public Vector3 location, handle_in, handle_out;
    public float distanceAlongPath = 0f;
    public List<string> tags;

    public BezierPoint(int _index, Vector3 _location, Vector3 _handle_in, Vector3 _handle_out)
    {
        index = _index;
        location = _location;
        handle_in = _handle_in;
        handle_out = _handle_out;
        tags = new List<string>();
    }

    public void SetHandles(Vector3 _distance)
    {
        _distance *= s_BezierHandleReach;
        handle_in = location - _distance;
        handle_out = location + _distance;
    }
}
