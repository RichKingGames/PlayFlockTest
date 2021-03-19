using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    private Tuple<Vector3, Vector3> _edges;
    private LineRenderer _lineRenderer;

    public void SetStartPoint(Vector3 pos)
    {
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        _edges = new Tuple<Vector3, Vector3>(pos, pos);
        _lineRenderer.SetPosition(0, pos);
        _lineRenderer.SetPosition(1, pos);
    }
    public void DrawConnection(Vector3 pos)
    {
        if(Vector3.Distance(_lineRenderer.GetPosition(1), pos) < .2f)
        {
            _lineRenderer.SetPosition(1, pos);
        }
        else
        {
            _lineRenderer.SetPosition(0, pos);
        }
       
    }
}
