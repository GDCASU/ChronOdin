using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowing_River : SimpleTimeManipulation
{
    public Vector3 forceDirection;
    public bool overrideDirection;
    public float streamForce;
    public float playerVelocity;
    private float originalPlayerSprint;
    private float originalPlayerWalk;
    public void Start() => UpdateTimeScale(MasterTime.singleton.timeScale);

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>())
            other.gameObject.GetComponent<Rigidbody>().velocity += (overrideDirection) ? forceDirection : transform.forward * streamForce * _timeScale;
        if (other.gameObject.tag == "Player")
        {
            if (_timeScale == 2 || _timeScale == 1)
            {
                PlayerController.singleton.baseMovementVariables.maxWalkVelocity = playerVelocity;
                PlayerController.singleton.baseMovementVariables.maxSprintVelocity = playerVelocity;
            }
            else
            {
                PlayerController.singleton.baseMovementVariables.maxWalkVelocity = originalPlayerWalk;
                PlayerController.singleton.baseMovementVariables.maxSprintVelocity = originalPlayerSprint;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            originalPlayerSprint = PlayerController.singleton.baseMovementVariables.maxSprintVelocity;
            originalPlayerWalk = PlayerController.singleton.baseMovementVariables.maxWalkVelocity;           
            PlayerController.singleton.ToggleGravity(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController.singleton.ToggleGravity(true);
            PlayerController.singleton.baseMovementVariables.maxWalkVelocity = originalPlayerWalk;
            PlayerController.singleton.baseMovementVariables.maxSprintVelocity = originalPlayerSprint;
        }
    }
}
