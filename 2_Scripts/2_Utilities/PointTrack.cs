using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTrack : MonoBehaviour
{
    [SerializeField] Color _lineColor = Color.yellow;

    Transform[] _points;

    void OnDrawGizmos()
    {
        Gizmos.color = _lineColor;
        _points = GetComponentsInChildren<Transform>();
        int nextIdx = 1;
        Vector3 currPos = _points[nextIdx].position;
        Vector3 nextPos;
        for(int n = 0; n <= _points.Length; n++)
        {
            nextPos = (++nextIdx >= _points.Length) ? _points[1].position : _points[nextIdx].position;
            Gizmos.DrawLine(currPos, nextPos);
            currPos = nextPos;
        }
    }
}
