using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private Transform playerBody;

    [Range(1f, 1000f)]
    public float mouseSensitivity = 1000f;

    public new bool enabled = true;

    private Camera mainCamera;

    private float rotationY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!enabled)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        if (mouseX != 0f)
        {
            rotationY += mouseX;
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
