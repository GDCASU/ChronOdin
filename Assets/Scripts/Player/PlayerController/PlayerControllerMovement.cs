using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    [System.Serializable]
    public class BaseMovementVariables
    {
        #region Variables

        #region General
        [Header("General")]
        public float maxSlope = 60;
        public float groundCheckDistance;
        #endregion

        #region Acceleration
        [Header("Acceleration")]
        public float walkSpeedIncrease = 1;
        public float sprintSpeedIncrease = 2;
        #endregion

        #region Velocity Caps
        [Header("Velocity Boundaries")]
        public float maxWalkVelocity = 7.5f;
        public float maxSprintVelocity = 15;
        public float minVelocity = .1f;
        #endregion

        #region Friction
        [Header("Friction Values")]
        public float groundFriction = .1f;
        public float inAirFriction = .004f;
        #endregion

        #region In Air
        [Header("In Air Variables")]
        [Range(0, 1)]
        public float inAirControl = .021f;
        #endregion

        #region Gravity
        [Header("Gravity Variables")]
        public float initialGravity = -.55f;
        public float gravityRate = 1.008f;
        public float maxGravity = -39.2f;
        #endregion

        #region Fake Ground Checks
        [Header("Fake Ground Variables")]
        public float fakeGroundTime = .1f;
        [HideInInspector] public float _fakeGroundTimer;
        [HideInInspector] public bool feetSphereCheck;
        [HideInInspector] public bool kneesCheck;
        #endregion

        #endregion

        public void StartVariables(CapsuleCollider capCollider)=> groundCheckDistance = capCollider.height * .5f - capCollider.radius;       
    }
    private void MovementInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            if (crouchMechanic) isSprinting = (crouchVariables.isCrouching ? false : true);
            else isSprinting = true;

        speedIncrease = (isSprinting) ? baseMovementVariables.sprintSpeedIncrease : baseMovementVariables.walkSpeedIncrease;
        maxVelocity = (isSprinting) ? baseMovementVariables.maxSprintVelocity : baseMovementVariables.maxWalkVelocity;

        if (Input.GetKey(KeyCode.W)) z = speedIncrease;
        else if (Input.GetKey(KeyCode.S)) z = -speedIncrease;
        else z = 0;
        if (Input.GetKey(KeyCode.D)) x = speedIncrease;
        else if (Input.GetKey(KeyCode.A)) x = -speedIncrease;
        else x = 0;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Time.timeScale == 1f) Time.timeScale = .1f;
            else Time.timeScale = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Q)) Time.timeScale = 0;
    }
    private void GroundCheck()
    {
        if (jumpMechanic)
        {
            if (_coyoteTimer > 0) _coyoteTimer -= Time.fixedDeltaTime;
            if (jumpVariables.justJumpedCooldown > 0) _justJumpedCooldown -= Time.fixedDeltaTime;
        }
        groundCheck = (!jumpMechanic || _justJumpedCooldown <= 0) ? Physics.SphereCast(transform.position, capCollider.radius, -transform.up, out hit, baseMovementVariables.groundCheckDistance + 0.01f) : false;
        surfaceSlope = Vector3.Angle(hit.normal, Vector3.up);
        if (surfaceSlope > baseMovementVariables.maxSlope)
        {
            groundCheck = false;
            if (playerState != PlayerState.Climbing && playerState != PlayerState.Jumping && playerState != PlayerState.InAir)
            {
                previousState = playerState;
                playerState = PlayerState.InAir;
                g = baseMovementVariables.initialGravity;
            }
        }
        totalVelocityToAdd = Vector3.zero;
        newForwardandRight = Vector3.zero;

        groundedForward = Vector3.Cross(hit.normal, -transform.right);
        groundedRight = Vector3.Cross(hit.normal, transform.forward);

        //Change the value of the groundcheck if the player is on the fakeGround state
        if (onFakeGround)
        {
            if (groundCheck) onFakeGround = false;
            else
            {
                groundCheck = true;
                groundedForward = transform.forward;
                groundedRight = transform.right;
            }
        }
        //Player just landed
        if (groundCheck && (playerState == PlayerState.Jumping || playerState == PlayerState.InAir || playerState == PlayerState.Climbing))
        {
            rb.velocity = rb.velocity - Vector3.up * rb.velocity.y;
            float angleOfSurfaceAndVelocity = Vector3.Angle(rb.velocity, (hit.normal - Vector3.up * hit.normal.y));
            if (!onFakeGround && hit.normal.y != 1 && angleOfSurfaceAndVelocity < 5 && z > 0)
                rb.velocity = (groundedRight * x + groundedForward * z).normalized * rb.velocity.magnitude;          //This is to prevent the weird glitch where the player bounces on slopes if they land on them without jumping
            friction = baseMovementVariables.groundFriction;
            _inAirJumps = jumpVariables.inAirJumps;
            previousState = playerState;
            playerState = PlayerState.Grounded;
            if (playerJustLanded != null) playerJustLanded();
            PlayerLanded();
            g = 0;
        }
        //Player just left the ground
        if (isGrounded && !groundCheck)
        {
            if (playerState != PlayerState.Jumping)
            {
                previousState = playerState;
                playerState = PlayerState.InAir;
                SetInitialGravity();
            }
            friction = baseMovementVariables.inAirFriction;
            _coyoteTimer = jumpVariables.coyoteTime;
            if (playerLeftGround != null) playerLeftGround();
        }
        isGrounded = groundCheck;

        //If close to a small step, raise the player to the height of the step for a smoother feeling movement
        float maxDistance = capCollider.radius * (1 + ((isSprinting) ? (rb.velocity.magnitude / baseMovementVariables.maxSprintVelocity) : 0));
        if (playerState == PlayerState.Grounded) baseMovementVariables.feetSphereCheck = Physics.SphereCast(transform.position - Vector3.up * .5f, capCollider.radius + .01f, rb.velocity.normalized, out feetHit, maxDistance);
        if (baseMovementVariables.feetSphereCheck && !onFakeGround)
        {
            Vector3 direction = feetHit.point - (transform.position - Vector3.up * .5f);
            float dist = direction.magnitude;
            baseMovementVariables.kneesCheck = Physics.Raycast(transform.position - Vector3.up * capCollider.height * .24f, (direction - rb.velocity.y * Vector3.up), dist);
            if (!baseMovementVariables.kneesCheck && playerState == PlayerState.Grounded && (x != 0 || z != 0))
            {
                StartCoroutine(FakeGround());
                isGrounded = true;
            }
        }

        if (!isGrounded)                                                                        //This is just for the downward launch, should be removed for jsut mvoement script 
        {
            Physics.Raycast(transform.position, -transform.up, out rayToGround, 50);
            distanceToGround = rayToGround.distance;
            timeSinceGrounded += Time.fixedDeltaTime;
        }
    }
    private void Move()
    {
        currentForwardAndRight = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (!isGrounded)
        {
            if (playerState != PlayerState.Climbing && playerState != PlayerState.Vaulting)
            {
                rb.velocity -= currentForwardAndRight * friction;

                newForwardandRight = (transform.right * x + transform.forward * z);
                if (z != 0 || x != 0)
                {
                    rb.velocity = newForwardandRight.normalized * currentForwardAndRight.magnitude * airControl + currentForwardAndRight * (1f - airControl) + rb.velocity.y * Vector3.up;
                }
            }
        }
        else
        {
            newForwardandRight = (groundedRight.normalized * x + groundedForward.normalized * z);
            if (hit.normal.y == 1)
            {
                newForwardandRight = new Vector3(newForwardandRight.x, 0, newForwardandRight.z);
                rb.velocity = (rb.velocity - Vector3.up * rb.velocity.y).normalized * rb.velocity.magnitude;
            }

            if (rb.velocity.magnitude < maxVelocity)
            {
                totalVelocityToAdd += newForwardandRight;
            }
            else if (playerState != PlayerState.Sliding)
            {
                if ((z == 0 && x == 0) || (pvX < 0 && x > 0)
                    || (x < 0 && pvX > 0) || (pvZ < 0 && z > 0)
                    || (z < 0 && pvZ > 0)) rb.velocity *= .99f; //If the palyer changes direction when going at the maxSpeed then decrease speed for smoother momentum shift
                else if (rb.velocity.magnitude < maxVelocity + 1f) rb.velocity = newForwardandRight.normalized * maxVelocity;
                totalVelocityToAdd = Vector3.zero;
            }

            if (rb.velocity.magnitude != maxVelocity || (x == 0 && z == 0))
            {
                totalVelocityToAdd -= rb.velocity * friction;
            }

            pvX = x;
            pvZ = z;
        }
    }
    public void SetInitialGravity() => g = baseMovementVariables.initialGravity;
    private void ApplyGravity()
    {
        if (playerState != PlayerState.Climbing)
        {
            if (!isGrounded)
            {
                totalVelocityToAdd += Vector3.up * g;
            }
            if (g > baseMovementVariables.maxGravity) g *= baseMovementVariables.gravityRate;
        }
    }
    private IEnumerator FakeGround()
    {
        onFakeGround = true;
        transform.position = new Vector3(transform.position.x, feetHit.point.y + 1f, transform.position.z);
        g = 0;
        baseMovementVariables._fakeGroundTimer = baseMovementVariables.fakeGroundTime;
        while (baseMovementVariables._fakeGroundTimer > 0 && onFakeGround)
        {
            baseMovementVariables._fakeGroundTimer -= Time.fixedDeltaTime;
            yield return fixedUpdate;
        }
        onFakeGround = false;
    }
    public void ResetPosition()
    {
        rb.velocity = Vector3.zero;
        g = 0;
        transform.position = lastViablePosition;
    }
    private void PlayerLanded()
    {
        climbVariables._climbingCooldown = 0;
        lastViablePosition = transform.position;
        timeSinceGrounded = 0;
        downLungeVariables.lungedUsed = false;
        launchVariables.abilityUsed = false;
    }
}
