/*
 * Revision Author: Cristion Dominguez
 * Revision Date: 17 Sept. 2021
 * Modification:
 *   Added a variable for determining the time an object should be slowed for.
 *   Added a variable for determining how slow the object should be when slowed.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float mouseSensitivity = 100f;
    private float moveSpeed = 12f;
    [SerializeField] Transform playerBody;
    [SerializeField] CharacterController controller;
    [SerializeField] Camera cam;

    private float range = 100f;
    private float xRotation = 0f;
    private float nextTimeToCastSl = 0f;
    private float nextTimeToCastSp = 0f;
    private float castRate = 5;

    private float slowObjectTime = 5f;
    private float slowDownFactor = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //mouse look rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //player move rotation
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        xRotation -= mouseY;
        //limiting mouse look
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //setting mouse rotation on player
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //rotating player body
        playerBody.Rotate(Vector3.up * mouseX);
        //setting the player facing vector based on input
        Vector3 move = transform.right * x + transform.forward * z;
        //send vector to character controller after scaling magnitude by moveSpeed once per frame
        controller.Move(move * moveSpeed * Time.deltaTime);

        //grab input for casting slowdown
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToCastSl)
        {
            //cooldown management
            nextTimeToCastSl = Time.time + 1f / castRate;
            CastSlowDown();
        }

        if (Input.GetButton("Fire2") && Time.time >= nextTimeToCastSp)
        {
            //cooldown management
            nextTimeToCastSp = Time.time + 1f / castRate;
            CastSpeedUp();
        }
    }
    private void CastSpeedUp()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            //raycast grabs game object of the target
            GameObject target = hit.transform.gameObject;
            if (target != null && target.CompareTag("targetObject"))
            {
                //invoke Speedup method on the target if its not already being sped up
                if(!target.GetComponent<SlowDownSpeedUpObject>().GetSpeedingStatus())
                    target.GetComponent<SlowDownSpeedUpObject>().SpeedUp();
            }
        }
    }
    private void CastSlowDown()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            //raycast grabs game object of the target
            GameObject target = hit.transform.gameObject;
            if (target != null && target.CompareTag("targetObject"))
            {
                //invoke SlowDown method on the target if not already slowing
                if (!target.GetComponent<SlowDownSpeedUpObject>().GetSlowingStatus())
                    target.GetComponent<SlowDownSpeedUpObject>().SlowDown(slowObjectTime, slowDownFactor);
            }
        }
    }
}
