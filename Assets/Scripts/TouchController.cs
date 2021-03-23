using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class that controls mouse inputs.
/// </summary>
public class TouchController : MonoBehaviour
{
    [SerializeField] private FieldController _fieldController;

    private Vector3 _mousePos;

    private float _doubleClickTimeLimit = 0.25f;
    private float _timerForDoubleClick;

    private bool _oneClick = false;


    private void Update()
    {
        _mousePos = ClickPositionByRay(); // get the position of the mouse in the world.
        CheckInput();
    }

    /// <summary>
    /// The method that controls double or single click.
    /// </summary>
    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_oneClick) // first click no previous clicks
            {
                _oneClick = true;
                _timerForDoubleClick = Time.time; // save the current time
                _fieldController.StartRectangle(_mousePos);
                return;
            }
            else
            {
                _oneClick = false; // found a double click, now reset
                DoubleClick();
            }
        }

        if (_oneClick)
        {
            // if the time now is delay seconds more than when the first click started. 
            if ((Time.time - _timerForDoubleClick) > _doubleClickTimeLimit)
            {
                _oneClick = false;
            }
        }

        SingleClick(); // invoke to reading input when button pressed or let go.
    }

    /// <summary>
    /// The method that invoked when single click.
    /// </summary>
    private void SingleClick()
    {

        if (Input.GetMouseButton(0))
        {
            _fieldController.MoveRectangle(_mousePos); 
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _fieldController.StopMoveRectangle();
        }


        if (Input.GetMouseButtonDown(1))
        {
            _fieldController.StartConnection(_mousePos); // create connection
        }
        else if (Input.GetMouseButton(1))
        {
            _fieldController.MoveConnectionWhenInit(_mousePos); // moving connection when we drugging it to another rectangle
        }
        else if (Input.GetMouseButtonUp(1))
        {
            _fieldController.EndConnection(_mousePos);  // finishing the creation of the connection
        }
    }

    /// <summary>
    /// The method that invoked when double click.
    /// </summary>
    private void DoubleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _fieldController.DoubleClickRectangle(_mousePos); // delete rectangle by mouse position.
        }
        else if (Input.GetMouseButtonDown(1))
        {
            _fieldController.DoubleClickConnection(_mousePos); // delete connection by mouse position.
        }
    }

    /// <summary>
    /// The method that returns Vector3 from a touch of your finger/mouse by using Raycast.
    /// </summary>
    private Vector3 ClickPositionByRay()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

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
