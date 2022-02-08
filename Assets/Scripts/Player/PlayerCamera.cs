using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float mouseSensitvity = 100f;
    public float camHeight = .75f;
    public Transform player;

    public Transform horizontalRotationHelper;
    float horizontalAngularVelocity;
    float verticalAngularVelocity;
    public float smoothTime = .02f;

    float xRotationHelper;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        transform.localRotation = player.transform.rotation;

        if (transform.parent) transform.parent = null;
        transform.localRotation = player.localRotation;

        horizontalRotationHelper.parent = null;
        horizontalRotationHelper.localRotation = transform.localRotation;
        xRotationHelper = transform.eulerAngles.x;

    }

    private void Update()
    {
        horizontalRotationHelper.position = transform.position;
        transform.position = player.position + player.up * camHeight;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitvity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitvity * Time.deltaTime;

        mouseX = HorizontalRotation(mouseX);
        mouseY = VerticalRotation(mouseY);
        transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0f);

    }

    public float HorizontalRotation(float mouseX)
    {
        horizontalRotationHelper.Rotate(Vector3.up * mouseX, Space.Self);
        float angle = Mathf.SmoothDampAngle(
            transform.localEulerAngles.y, horizontalRotationHelper.localEulerAngles.y, ref horizontalAngularVelocity, smoothTime);
        return angle;
    }
    public float VerticalRotation(float mouseY)
    {
        xRotationHelper = Mathf.Clamp(xRotationHelper - mouseY, -90, 90);
        float angle = Mathf.SmoothDampAngle(
            transform.localEulerAngles.x, xRotationHelper, ref verticalAngularVelocity, smoothTime);
        return angle;
    }

    public void AdjustCameraHeight(bool moveDown, float cameraDisplacement)
    {
        if (moveDown) camHeight -= cameraDisplacement;
        else camHeight += cameraDisplacement;
    }
}
