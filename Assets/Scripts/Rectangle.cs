using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RectangleState
{
    Idle,
    Moving
}
public class Rectangle : MonoBehaviour
{
    private RectangleState _state;

    private int _id;

    void Start()
    {
        SetState(RectangleState.Idle);
        SetRandomColor();
    }

    public void SetId(int id)
    {
        _id = id;
    }
    public int GetId()
    {
        return _id;
    }
    public void SetState(RectangleState state)
    {
        _state = state;
    }
    private void SetRandomColor()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material material = renderer.material;
        material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    }
}
