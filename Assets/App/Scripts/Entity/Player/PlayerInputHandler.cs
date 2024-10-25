using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInputHandler : MonoBehaviour
{
    public float sensitivity = 100; 

    public Vector2 GetMoveDirection()
    {
        Vector2 dir = Vector2.zero;
        if (Input.GetKey(KeyCode.W)){
            dir.y = 1;
        } else if (Input.GetKey(KeyCode.S))
        {
            dir.y = -1;
        }

        if (Input.GetKey(KeyCode.D)){
            dir.x = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            dir.x = -1;
        }

        return dir.normalized;
    }

    public Vector2 GetMouseDeltaRotation(){
        Vector2 mouseRotationDelta =Vector2.zero;
        float yRotation = Input.GetAxis("Mouse X");
        float xRotation = Input.GetAxis("Mouse Y");

        mouseRotationDelta.x = xRotation * sensitivity ;
        mouseRotationDelta.y = yRotation * sensitivity;

        return mouseRotationDelta;
    }

    public bool GetSpaceHeld()
    {
        return Input.GetKey(KeyCode.Space);
    }

    public bool GetSpacePressed()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public bool GetShiftPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }    
    
    public bool GetCtrlPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftControl);
    }    
    
    public bool GetCtrlReleased()
    {
        return Input.GetKeyUp(KeyCode.LeftControl);
    }

    public bool GetCtrlHeld()
    {
        return Input.GetKey(KeyCode.LeftControl);
    }

    public bool GetLMBPressed(){
        return Input.GetMouseButtonDown(0);
    }

    public bool GetLMBReleased(){
        return Input.GetMouseButtonUp(0);
    }

    public bool GetLMBHeld(){
        return Input.GetMouseButton(0);
    }

    public bool GetRMBPressed()
    {
        return Input.GetMouseButtonDown(1);
    }

    public bool GetRMBReleased()
    {
        return Input.GetMouseButtonUp(1);
    }

    public bool GetRMBHeld()
    {
        return Input.GetMouseButton(1);
    }

    public bool GetQPress(){
        return Input.GetKeyDown(KeyCode.Q);
    }
}
