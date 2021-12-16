using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    [System.Serializable]
    public class LaunchVariables
    {
        public GameObject indicator;
        public float maxDistance = 50;
        public float timeToReachTarget = 2;
        public float launchForce = 2;
        [HideInInspector] public Vector3 launchDestination;
        [HideInInspector] public float cooldown = 2;
        [HideInInspector] public bool abilityUsed;
        [HideInInspector] public float xzFrictionCompesator;
        [HideInInspector] public float calculatedYVelocityLost = -15.25889f;
        public bool valideTarget;
    }
    private void LaunchInput()
    {
        RaycastHit hit;
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, launchVariables.maxDistance)) launchVariables.indicator.transform.position = hit.point;
            else launchVariables.indicator.transform.position = Vector3.zero;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1) && !launchVariables.abilityUsed)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, launchVariables.maxDistance))
            {
                launchVariables.launchDestination = launchVariables.indicator.transform.position;
                launchVariables.indicator.transform.position = Vector3.zero;
                launchVariables.valideTarget = true;
            }
            //else launchVariables.valideTarget = false;
        }
    }
    public void PerformLaunch()
    {
        launchVariables.valideTarget = false;
        Vector3 direction = (launchVariables.launchDestination - transform.position);
        float yDistance = direction.y;
        Vector3 forceVector = direction - Vector3.up * yDistance;
        forceVector /= launchVariables.xzFrictionCompesator;
        forceVector.y = yDistance - launchVariables.calculatedYVelocityLost;

        rb.velocity = forceVector + new Vector3(rb.velocity.x, 0, rb.velocity.z);
        launchVariables.abilityUsed = true;
    }
}
