using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed;

    CharacterController charControl; // controls camera movement
    Vector3 startPos;

    void Awake()
    {
        charControl = GetComponent<CharacterController>();
        startPos =transform.position;
        Debug.Log("Camera starts at " + startPos);
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        float horizontal = Input.GetAxis("Horizontal_ArrowKeys");
        float vertical = Input.GetAxis("Vertical_ArrowKeys");

        Vector3 moveDirSide = transform.right * horizontal * (moveSpeed * 0.01f);
        Vector3 moveDirFwd = transform.forward * vertical * (moveSpeed * 0.01f); // Unlike transform.forward, Vector3.forward doesn't count in rotation

        charControl.Move(moveDirSide);
        charControl.Move(moveDirFwd);
    }

    // NOT WORKING
    // reposition camera to start pos
    public void restartPos()
    {
        transform.position = startPos;
        Debug.Log("Camera repositioned at " + transform.position);
    }
}
