using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;

    public float mouseSensitivity = 1000f;
    public float rotationX = 0f;
    public float rotationY = 0f;

    private Camera mainCamera;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        // float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // if (mouseY != 0f)
        // {
        //     rotationX -= mouseY;
        //     rotationX = Mathf.Clamp(rotationX, -60f, 60f);
        // transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        // }

        if (mouseX != 0f)
        {
            rotationY += mouseX;
            playerBody.Rotate(Vector3.up * mouseX);
        }

        // playerBody.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
