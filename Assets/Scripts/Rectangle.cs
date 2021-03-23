using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rectangle : MonoBehaviour
{
    void Start()
    {
        SetRandomColor();
    }
    private void SetRandomColor()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material material = renderer.material;
        material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    }
}
