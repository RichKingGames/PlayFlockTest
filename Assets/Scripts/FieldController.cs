using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    [SerializeField] private GameObject _rectanglePrefab;
    [SerializeField] private GameObject _linePrefab;

    private Dictionary<Tuple<int, int>, GameObject> _connections;
    private List<Rectangle> _rectangles;
    private GameObject _currentRectangle;
    private int _startConnectionId;
    private Connection _currentLine;

    void Start()
    {
        _rectangles = new List<Rectangle>();
        _connections = new Dictionary<Tuple<int, int>, GameObject>();
    }

    public void CreateRectangle(Vector3 mousePos)
    {
        GameObject rectangle = ClickOnRectangle(mousePos);
        if (ClickOnRectangle(mousePos))
        {
            _currentRectangle.GetComponent<Rectangle>().SetState(RectangleState.Moving);
            _currentLine = FindConnection(_currentRectangle);
        }
        else
        {
            rectangle = Instantiate(_rectanglePrefab, mousePos, Quaternion.identity);
            rectangle.GetComponent<Rectangle>().SetId(_rectangles.Count);
            _rectangles.Add(rectangle.GetComponent<Rectangle>());
        }
    }
    public void MoveRectangle(Vector3 mousePos)
    {
        if (_currentRectangle != null)
        {
            Vector3 prevPos = _currentRectangle.transform.position;
            GameObject closest = FindClosestRectangle(_currentRectangle);

            if (RectangleIntersect(closest.transform.position,mousePos))
            {
                _currentRectangle.transform.position = prevPos;
            }
            else
            {
                _currentRectangle.transform.position = mousePos;
                _currentLine.DrawConnection(_currentRectangle.transform.position);
            }
            
            return;
        }
    }
    public void StopMoveRectangle()
    {
        if (_currentRectangle != null)
        {
            _currentRectangle.GetComponent<Rectangle>().SetState(RectangleState.Idle);
            _currentRectangle = null;
        }
    }

    public void StartConnection(Vector3 mousePos)
    {
        GameObject rectangle = ClickOnRectangle(mousePos);
        if(rectangle!=null)
        {
            GameObject line = Instantiate(_linePrefab, rectangle.transform.position, Quaternion.identity);
            line.GetComponent<Connection>().SetStartPoint(rectangle.transform.position);
            _currentLine = line.GetComponent<Connection>();
            _startConnectionId = rectangle.GetComponent<Rectangle>().GetId();
            _connections.Add(new Tuple<int, int>(_startConnectionId, _startConnectionId), line);
            
        }
    }

    public void MoveConnection(Vector3 mousePos)
    {
        if (_currentLine != null)
        {
            _currentLine.GetComponent<Connection>().DrawConnection(mousePos);
        }

    }

    private void DeleteConnection(Connection connection)
    {
        _connections.Remove(new Tuple<int, int>(_startConnectionId, _startConnectionId));
        Destroy(connection);
        _currentLine = null;
    }
    public void EndConnection(Vector3 mousePos)
    {
        if (_rectangles == null)
        {
            return;
        }

        int id1 = _currentRectangle.GetComponent<Rectangle>().GetId();
        GameObject line;
        foreach (var x in _rectangles)
        {
            if (RectangleIntersect(x.transform.position, mousePos))
            {
                foreach (var y in _connections)
                {
                    if (y.Key.Item1 == id1 || y.Key.Item2 == id1)
                    {
                        int id2 = x.GetComponent<Rectangle>().GetId();
                        line = y.Value;
                        _connections.Remove(y.Key);
                        _connections.Add(new Tuple<int, int>(id1, id2), line);
                        line.GetComponent<Connection>().DrawConnection(x.transform.position);
                        return;
                    }
                }
            }
        }
        
        DeleteConnection(_currentLine);

    }
    private Connection FindConnection(GameObject rectangle)
    {
        if(rectangle!=null)
        {
            int id = rectangle.GetComponent<Rectangle>().GetId();
            foreach(var x in _connections)
            {
                if (x.Key.Item1 == id || x.Key.Item2 == id)
                {
                    return x.Value.GetComponent<Connection>();
                }
            }
        }
        return null;
    }
    private GameObject ClickOnRectangle(Vector3 mousePos)
    {
        if (_rectangles == null)
        {
            return null;
        }

        foreach (var x in _rectangles)
        {
            if(RectangleIntersect(x.transform.position, mousePos))
            {
                _currentRectangle = x.gameObject;
                return x.gameObject;
            }
        }
        return null;

    }
    private bool RectangleIntersect(Vector3 a, Vector3 b)
    {
        return (Mathf.Abs(a.x - b.x) * 2 < (2 + 2)) &&
                (Mathf.Abs(a.z - b.z) * 2 < (1 + 1));
    }

    private GameObject FindClosestRectangle(GameObject rectangle)
    {
        GameObject closest = null;
        Vector3 diff;
        float distance = Mathf.Infinity;

        foreach (var go in _rectangles)
        {
            if(go.gameObject != rectangle)
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

        return closest;
    }
}
