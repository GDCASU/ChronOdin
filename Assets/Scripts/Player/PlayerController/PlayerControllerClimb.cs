using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    [System.Serializable]
    public class ClimbVariables
    {
        #region Climb
        [Header("Climbing Variables")]
        public float negativeVelocityToClimb = -45;
        public float climbingDuration = 1;
        [HideInInspector] public float _climbingTime;
        public float climbAcceleration = .5f;
        public float maxClimbingVelocity = 10;
        public float initialClimbingGravity = .5f;
        [HideInInspector] public float _climbingGravity;
        public float climbingGravityMultiplier = 1.005f;
        public float climbingStrafe = .3f;
        [HideInInspector] public float _climbingStrafe;
        public float climbStrafeDecreaser = .001f;
        public float maxClimbStrafeVelocity = 5;
        public float climbingStrafeFriction = .01f;
        public float endOfClimbJumpStrength = 3;
        public float endOfClimbJumpHeight = 4;
        public float climbingCooldown = 2;
        [HideInInspector] public float _climbingCooldown;
        #endregion

        #region WallJump
        [Header("WallJump Variables")]
        public float wallJumpHeightStrenght = 5;
        public float wallJumpNormalStrength = 5;
        #endregion
    }

    public void HandleClimb()
    {
        if (climbVariables._climbingCooldown > 0) climbVariables._climbingCooldown -= Time.fixedDeltaTime;
        if (playerState == PlayerState.InAir && vaultVariables.forwardCheck
            && rb.velocity.y > climbVariables.negativeVelocityToClimb
            && (z > 0 || currentForwardAndRight.magnitude > 0f)
            && climbVariables._climbingCooldown <= 0)
        {
            previousState = playerState;
            playerState = PlayerState.Climbing;
            StartCoroutine(ClimbCoroutine());
        }
    }
    private IEnumerator ClimbCoroutine()
    {
        climbVariables._climbingTime = climbVariables.climbingDuration;
        if (jumpMechanic) _justJumpedCooldown = 0;
        climbVariables._climbingGravity = climbVariables.initialClimbingGravity;
        Physics.BoxCast(transform.position - transform.forward.normalized * capCollider.radius * .5f, Vector3.one * capCollider.radius, transform.forward, out forwardHit, Quaternion.identity, 1f);
        climbVariables._climbingStrafe = climbVariables.climbingStrafe;
        Vector3 playerOnWallRightDirection = Vector3.Cross(forwardHit.normal, Vector3.up).normalized;
        Vector3 originalHorizontalClimbingDirection = Vector3.Project(velocityAtCollision, playerOnWallRightDirection);
        Vector3 upwardDirection = (surfaceSlope == 0) ? Vector3.up : -Vector3.Cross(hit.normal, playerOnWallRightDirection).normalized;
        rb.velocity = originalHorizontalClimbingDirection;
        while (!isGrounded && vaultVariables.forwardCheck && playerState == PlayerState.Climbing && climbVariables._climbingTime > 0)
        {
            if (_jumpBuffer > 0)
            {
                rb.velocity += Vector3.up * climbVariables.wallJumpHeightStrenght + forwardHit.normal * climbVariables.wallJumpNormalStrength;
                g = jumpVariables.jumpingInitialGravity;
                SetVariablesOnJump();
                climbVariables._climbingCooldown = climbVariables.climbingCooldown;
                previousState = playerState;
                playerState = PlayerState.InAir;
                yield break;
            }
            rb.velocity += upwardDirection.normalized * ((z > 0) ? (rb.velocity.y > climbVariables.maxClimbingVelocity ? 0 : climbVariables.climbAcceleration) : (originalHorizontalClimbingDirection.magnitude > 7.5f) ? 0 : -climbVariables._climbingGravity);
            rb.velocity += (currentForwardAndRight.magnitude < climbVariables.maxClimbStrafeVelocity) ? playerOnWallRightDirection * x * climbVariables._climbingStrafe : Vector3.zero - currentForwardAndRight * climbVariables.climbingStrafeFriction;
            climbVariables._climbingGravity *= climbVariables.climbingGravityMultiplier;
            climbVariables._climbingTime -= Time.fixedDeltaTime;
            climbVariables._climbingStrafe -= climbVariables.climbStrafeDecreaser;

            yield return fixedUpdate;
        }
        if (playerState == PlayerState.Vaulting) yield break;
        rb.velocity += Vector3.up * climbVariables.endOfClimbJumpHeight + playerOnWallRightDirection * x + forwardHit.normal * climbVariables.endOfClimbJumpStrength;
        climbVariables._climbingCooldown = climbVariables.climbingCooldown;
        previousState = playerState;
        if (!isGrounded) playerState = PlayerState.InAir;
        else rb.velocity = -Vector3.up * rb.velocity.y;
        SetInitialGravity();
        StartCoroutine(EndOfClimbAirControl());
    }
    private IEnumerator EndOfClimbAirControl()
    {
        airControl = jumpVariables.jumpInAirControl;
        yield return new WaitForSeconds(.5f);
        airControl = baseMovementVariables.inAirControl;
    }
}
