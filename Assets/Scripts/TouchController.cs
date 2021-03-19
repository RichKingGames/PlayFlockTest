using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    [SerializeField] private FieldController _fieldController;
    private Plane plane;

    void Update()
    {
        GetMouseInput();
    }

    private void GetMouseInput()
    {
        Vector3 mousePos = ClickPositionByRay();
        if (Input.GetMouseButtonDown(0))
        {
            _fieldController.CreateRectangle(mousePos);
        }
        else if (Input.GetMouseButton(0))
        {
            _fieldController.MoveRectangle(mousePos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _fieldController.StopMoveRectangle();
        }


        if (Input.GetMouseButtonDown(1))
        {
            _fieldController.StartConnection(mousePos);
        }
        else if (Input.GetMouseButton(1))
        {
            _fieldController.MoveConnection(mousePos);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            _fieldController.EndConnection(mousePos);
        }
    }
 
    
    /// <summary>
    /// Method that returns Vector3 from a touch of your finger/mouse by using Raycast.
    /// </summary>
    Vector3 ClickPositionByRay()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        plane = new Plane(Vector3.up, transform.position);

        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (plane.Raycast(mRay, out rayDistance))
        {
            pos = mRay.GetPoint(rayDistance);
            pos.y = 0.5f;
        }
        return pos;
    }
}
