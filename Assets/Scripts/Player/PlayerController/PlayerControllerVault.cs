using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    [System.Serializable]
    public class VaultVariables
    {
        public bool climbMechanic;

        #region Climbing Checks
        [HideInInspector] public bool feetSphereCheck;
        [HideInInspector] public bool kneesCheck;
        public float minClimbCheckDistance = .1f;
        public float maxClimbCheckDistance = .6f;
        public float minClimbSlope = 65;

        [HideInInspector] public bool feetCheck;
        [HideInInspector] public bool headCheck;
        [HideInInspector] public bool forwardCheck;
        #endregion

        #region Vault
        [Header("Vault Variables")]
        public float vaultClimbStrength = 10;
        public float vaultEndStrength = 6;
        public float vaultDuration = .8f;
        #endregion
    }
    public void ClimbChecks()
    {
        float maxDistance = capCollider.radius * (1 + ((isSprinting) ? (rb.velocity.magnitude / baseMovementVariables.maxSprintVelocity) : 0));
        if (playerState == PlayerState.Grounded) vaultVariables.feetSphereCheck = Physics.SphereCast(transform.position - Vector3.up * .5f, capCollider.radius + .01f, rb.velocity.normalized, out feetHit, maxDistance);

        vaultVariables.headCheck = Physics.Raycast(Camera.main.transform.position + Vector3.up * .25f, transform.forward, capCollider.radius + ((surfaceSlope >= vaultVariables.minClimbSlope) ? vaultVariables.maxClimbCheckDistance * 2 : vaultVariables.minClimbCheckDistance));
        vaultVariables.forwardCheck = Physics.Raycast(transform.position, transform.forward, capCollider.radius + ((surfaceSlope >= vaultVariables.minClimbSlope) ? vaultVariables.maxClimbCheckDistance : vaultVariables.minClimbCheckDistance));

        if (vaultVariables.forwardCheck && currentForwardAndRight.magnitude > 1)
        {
            velocityAtCollision = currentForwardAndRight;
            //if (playerState != PlayerState.Climbing) rb.velocity = Vector3.zero;              //Avoid bouncing
        }
        vaultVariables.kneesCheck = false;
        if (vaultVariables.climbMechanic) HandleClimb();
        HandleVault();
    }
    public void HandleVault()
    {
        if ((playerState == PlayerState.InAir || (playerState == PlayerState.Climbing && surfaceSlope == 0)) && vaultVariables.forwardCheck && !vaultVariables.headCheck && z > 0)
        {
            previousState = playerState;
            playerState = PlayerState.Vaulting;
            StartCoroutine(VaultCoroutine());
        }

    }
    private IEnumerator VaultCoroutine()
    {
        rb.velocity = Vector3.up * vaultVariables.vaultClimbStrength;
        float height = Camera.main.transform.position.y;
        Physics.BoxCast(transform.position - transform.forward.normalized * capCollider.radius * .5f, Vector3.one * capCollider.radius, transform.forward, out forwardHit, Quaternion.identity, 1f);
        vaultVariables.feetCheck = (Physics.Raycast(transform.position - Vector3.up * capCollider.height * .5f, transform.forward, capCollider.radius + .1f));
        while ((transform.position.y - capCollider.height * .5) < height && rb.velocity.y > 0)
        {
            rb.velocity += .05f * Vector3.up;
            yield return fixedUpdate;
        }
        vaultVariables.feetCheck = false;
        previousState = playerState;
        if (!isGrounded) playerState = PlayerState.InAir;
        rb.velocity = ((forwardHit.normal.magnitude == 0) ? transform.forward : -forwardHit.normal) * vaultVariables.vaultEndStrength;
    }

}
