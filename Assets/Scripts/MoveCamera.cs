using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float mouseSensitvity = 100f;

    public Transform player;
    public Transform camera;

    float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitvity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitvity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90f);

        camera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        player.Rotate(Vector2.up * mouseX);
    }

    public void AdjustCameraHeight(bool moveDown)
    {
        if (moveDown) camera.transform.position -= Vector3.up;
        else camera.transform.position += Vector3.up;
    }
}
