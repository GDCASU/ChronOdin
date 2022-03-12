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
    public bool isLinked;
    public void Start() => UpdateWithGlobalTimescale(MasterTime.singleton.timeScale);

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>())
            other.gameObject.GetComponent<Rigidbody>().velocity += ((overrideDirection) ? forceDirection : transform.forward) * streamForce * timeScale;
        if (other.gameObject.tag == "Player")
        {
            if (timeScale == 1)
            {
                PlayerController.singleton.ChangeWalkingSpeed(playerVelocity);
                PlayerController.singleton.ChangeSprintSpeed(playerVelocity);
            }
            else
            {
                PlayerController.singleton.ResetWalkingSpeed();
                PlayerController.singleton.ResetSprintSpeed();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController.singleton.ChangeWalkingSpeed(playerVelocity);
            PlayerController.singleton.ChangeSprintSpeed(playerVelocity);

        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController.singleton.ResetWalkingSpeed();
            PlayerController.singleton.ResetSprintSpeed();
        }
        
    }
    public override void ActivateSingleObjectEffect(float activeTime, TimeEffect effect)
    {
        if (isLinked) GetComponentInParent<LinkFlowingRivers>().ActivateAllLinkedRivers(activeTime, effect);
        else StartCoroutine(SingleObjectEffect(activeTime, effect));
    }
    public void ActivateLinkedRiverEffect(float activeTime, TimeEffect effect)
    {
        StartCoroutine(SingleObjectEffect(activeTime, effect));
    }
}
