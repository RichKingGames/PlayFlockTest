using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class of the connection.
/// </summary>
public class Connection : MonoBehaviour
{
    private List<GameObject> _rectangles; // list contains attached rectangles.
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    /// <summary>
    /// The method that attached rectangles to connection.
    /// </summary>
    public void SetRectangles(List<GameObject> rectangles)
    {
        _rectangles = rectangles;
    }

    /// <summary>
    /// The method that checks if the connection has this rectangle.
    /// </summary>
    public bool HasRectangle(GameObject rectangle)
    {
        for(int i = 0; i < _rectangles.Count; i++)
        {
            if(_rectangles[i] == rectangle)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The method that visualizing the connection from the rectangle to the mouse position.
    /// </summary>
    public void DrawByMousePos(Vector3 mousePos)
    {
        _lineRenderer.SetPosition(0, _rectangles[0].transform.position);
        _lineRenderer.SetPosition(1, mousePos);
    }

    /// <summary>
    /// The method that visualizing the connection by attached rectangles.
    /// </summary>
    public void DrawByGameObjects()
    {
        for(int i = 0; i < _rectangles.Count; i++)
        {
            _lineRenderer.SetPosition(i, _rectangles[i].transform.position);
        }
    }
}
