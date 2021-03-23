using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main class that controls all action on the field.
/// </summary>
public class FieldController : MonoBehaviour
{
    [SerializeField] private GameObject _rectanglePrefab;
    [SerializeField] private GameObject _linePrefab;

    private List<Rectangle> _rectangles; // list of all the rectangles on the field.
    private List<Connection> _connections; // list of all the connections on the field.

    private Rectangle _currentRectangle; // the object contains the rectangle that you are currently holding or dragging the connection from.
    private Connection _currentConnection; // the object contains the connection you are currently dragging.


    void Start()
    {
        _rectangles = new List<Rectangle>();
        _connections = new List<Connection>();
    }

    /// <summary>
    /// The method that controls whether you click on a rectangle or on a void.
    /// </summary>
    public void StartRectangle(Vector3 mousePos)
    {
        if(ClickOnRectangle(mousePos)) // if click on the rectangle.
        {
            SetCurrentRectangle(mousePos);
        }
        else // if click on empty space.
        {
            CreateRectangle(mousePos);
        }
    }

    /// <summary>
    /// The method that moves the rectangle and updates the visualization of the connection.
    /// </summary>
    public void MoveRectangle(Vector3 mousePos)
    {
        if (_currentRectangle != null)
        {
            Vector3 prevPos = _currentRectangle.transform.position;

            GameObject closest = FindClosestRectangle(_currentRectangle.gameObject); // finding closest rectangle to current rectangle
            SetCurrentConnection(FindConnectionByRectangle(_currentRectangle)); // setting current connection by current rectangle.
            
            if (RectangleIntersect(closest.transform.position, mousePos)
                && closest !=_currentRectangle.gameObject) // if mouse position intersects with a rectangle.
            {
                _currentRectangle.transform.position = prevPos;
            }
            else // if mouse position don't intersects with a rectangle.
            {
                _currentRectangle.transform.position = mousePos;
                if(_currentConnection!=null)
                {
                    _currentConnection.DrawByGameObjects(); // updates visualization of the connection.
                }
            }
        }
    }

    /// <summary>
    /// The method that nullifies the current connection and rectangle. Invokes when LeftClick let go.
    /// </summary>
    public void StopMoveRectangle()
    {
        DeleteCurrentRectangle();
        DeleteCurrentConnection();
    }

    /// <summary>
    /// The method that controls whether the rectangle has a connection.
    /// </summary>
    public void StartConnection(Vector3 mousePos)
    {
        if(ClickOnRectangle(mousePos)) // checking are we click on rectangle.
        {
            SetCurrentRectangle(mousePos); 

            if(FindConnectionByRectangle(_currentRectangle)) // if the current rectangle has a connection, destroy it and create a new one.
            {
                Connection connection = FindConnectionByRectangle(_currentRectangle);
                DestroyConnection(connection);
                CreateConnection(mousePos);
            }
            else // if the current rectangle hasnt a connection. Creating a new one.
            {
                CreateConnection(mousePos);
            }

        }

    }

    /// <summary>
    /// The method that create the new connection.
    /// </summary>
    public void CreateConnection(Vector3 mousePos)
    {
        GameObject connection = Instantiate(_linePrefab, mousePos, Quaternion.identity); // instantiate on scene line prefab.
        SetCurrentConnection(connection.GetComponent<Connection>());
        _currentConnection.SetRectangles(new List<GameObject> { _currentRectangle.gameObject }); // setting current rectangle to connection.
        _currentConnection.DrawByMousePos(mousePos); // visualizing connection from rectangle to the mouse position.
        _connections.Add(_currentConnection);
    }

    /// <summary>
    /// The method visualizing the connection from the rectangle to the mouse position. Invokes when RightClick pressed.
    /// </summary>
    public void MoveConnectionWhenInit(Vector3 mousePos)
    {
        if(_currentConnection!=null)
        {
            _currentConnection.DrawByMousePos(mousePos); // visualizing connection from rectangle to the mouse position.
        }
    }

    /// <summary>
    /// The method that attaches connection to another rectangle by mouse position. Invokes when RightClick let go.
    /// </summary>
    public void EndConnection(Vector3 mousePos)
    {
        for(int i=0; i<_rectangles.Count; i++)
        {
            if(RectangleIntersect(_rectangles[i].transform.position, mousePos)) // if mouse position on rectangle.
            {
                
                if(FindConnectionByRectangle(_rectangles[i])) // if this rectangle have another connection
                {
                    // delete another connection.
                    Connection connection = FindConnectionByRectangle(_rectangles[i]);
                    DestroyConnection(connection); 

                    // attach the new connection
                    _currentConnection.SetRectangles(new List<GameObject> { _currentRectangle.gameObject, _rectangles[i].gameObject });
                    _currentConnection.DrawByGameObjects();
                    DeleteCurrentConnection();
                    DeleteCurrentRectangle();
                    return;
                }
                else // if this rectangle hasn't a connection.
                {
                    _currentConnection.SetRectangles(new List<GameObject> { _currentRectangle.gameObject, _rectangles[i].gameObject });
                    _currentConnection.DrawByGameObjects();
                    DeleteCurrentConnection();
                    DeleteCurrentRectangle();
                    return;
                }
                
            }
        }
        // if mouse position at empty space.
        DestroyConnection(_currentConnection);
        DeleteCurrentConnection();
        DeleteCurrentRectangle();
    }

    /// <summary>
    /// The method that deleting rectangle. Invokes by left double click.
    /// </summary>
    public void DoubleClickRectangle(Vector3 mousePos)
    {
        if(ClickOnRectangle(mousePos)) // if mouse position on rectangle.
        {
            Rectangle rectangle = FindRectangleByMouse(mousePos);
            Connection connection = FindConnectionByRectangle(rectangle);
            _rectangles.Remove(rectangle);
            Destroy(rectangle.gameObject); // delete the rectangle.
            DestroyConnection(connection); // delete the connection.
        }
    }

    /// <summary>
    /// The method that deleting connection. Invokes by right double click.
    /// </summary>
    public void DoubleClickConnection(Vector3 mousePos)
    {
        if(ClickOnRectangle(mousePos)) // if mouse position on rectangle.
        {
            Rectangle rectangle = FindRectangleByMouse(mousePos); // returns rectangle by mouse position.
            Connection connection = FindConnectionByRectangle(rectangle); // returns connection by rectangle.
            DestroyConnection(connection); // delete the connection.
        }
    }

    /// <summary>
    /// The method that checks for a click on a rectangle.
    /// </summary>
    private bool ClickOnRectangle(Vector3 mousePos)
    {
        if (_rectangles == null)
        {
            return false;
        }

        foreach (var x in _rectangles)
        {
            if (RectangleIntersect(x.transform.position, mousePos)) // if mouse position and rectangle intersect.
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// The method that finds a rectangle by mouse position.
    /// </summary>
    private Rectangle FindRectangleByMouse(Vector3 mousePos)
    {
        for (int i = 0; i < _rectangles.Count;i++)
        {
            if(RectangleIntersect(_rectangles[i].transform.position, mousePos)) // if mouse position and rectangle intersect.
            {
                return _rectangles[i];
            }
        }
        return null;
    }

    /// <summary>
    /// The method that finds a connection by a rectangle.
    /// </summary>
    private Connection FindConnectionByRectangle(Rectangle rectangle)
    {
        for (int i = 0; i < _connections.Count; i++)
        {
            if (_connections[i].HasRectangle(rectangle.gameObject)) // if the connection has a rectangle.
            {
                return _connections[i];
            }
        }
        return null;
    }

    /// <summary>
    /// The method that deletes the connection from the list and destroys a gameobject.
    /// </summary>
    private void DestroyConnection(Connection connection)
    {
        if(connection!=null)
        {
            _connections.Remove(connection);
            Destroy(connection.gameObject);
        }
    }

    /// <summary>
    /// The method that sets the current rectangle(which holding now).
    /// </summary>
    private void SetCurrentRectangle(Vector3 mousePos)
    {
        foreach (var x in _rectangles)
        {
            if (RectangleIntersect(x.transform.position, mousePos))
            {
                _currentRectangle = x;
            }
        }
    }

    /// <summary>
    /// The method that null the current rectangle.
    /// </summary>
    private void DeleteCurrentRectangle()
    {
        _currentRectangle = null;
    }

    /// <summary>
    /// The method that sets the current connection (which holding now).
    /// </summary>
    private void SetCurrentConnection(Connection connection)
    {
        _currentConnection = connection;
    }

    /// <summary>
    /// The method that null the current connection.
    /// </summary>
    private void DeleteCurrentConnection()
    {
        _currentConnection = null;
    }

    /// <summary>
    /// The method that instantiate rectangle on scene.
    /// </summary>
    private void CreateRectangle(Vector3 mousePos)
    {
        GameObject rectangle = Instantiate(_rectanglePrefab, mousePos, Quaternion.identity);
        _rectangles.Add(rectangle.GetComponent<Rectangle>());
    }

    /// <summary>
    /// The method that tests the intersection of two rectangles.
    /// </summary>
    private bool RectangleIntersect(Vector3 a, Vector3 b)
    {
        return (Mathf.Abs(a.x - b.x) * 2 < (2 + 2)) &&
                (Mathf.Abs(a.z - b.z) * 2 < (1 + 1));
    }

    /// <summary>
    /// The method that finding the closest rectangle by the current rectangle.
    /// </summary>
    private GameObject FindClosestRectangle(GameObject rectangle)
    {
        GameObject closest = null;
        Vector3 diff;
        float distance = Mathf.Infinity;

        foreach (var go in _rectangles)
        {
            if (go.gameObject != rectangle)
            {
                diff = go.gameObject.transform.position - rectangle.transform.position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go.gameObject;
                    distance = curDistance;
                }
            }
        }
        if(closest!=null)
        {
            return closest;
        }
        return rectangle;
    }
}
