using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveThree : MonoBehaviour
{
    #region Variables
    public enum PlayerState
    {
        NotMoving,
        Grounded,
        Sliding,
        Jumping,
        Climbing,
        Vaulting,
        InAir,
    };

    #region Components
    private Rigidbody rb;
    private CapsuleCollider capCollider;
    private MoveCamera moveCamera;
    #endregion

    #region Primitive Variables
    float x, z;
    float pvX, pvZ;
    float y, g;

    float scrollWheelDelta;
    float groundCheckDistance;
    #endregion

    #region Player States
    [Header("Player States")]
    public bool isGrounded;
    bool groundCheck;
    public bool isSprinting;
    public bool isCrouching;
    public bool onFakeGround;
    public PlayerState playerState;
    public PlayerState previousState;
    #endregion

    #region General
    [Header("General")]
    public float maxSlope;
    private float slope;
    #endregion

    #region Acceleration
    [Header("Acceleration")]
    public float walkSpeedIncrease;
    public float sprintSpeedIncrease;
    public float speedIncrease;
    #endregion

    #region Velocity Caps
    [Header("Velocity Boundaries")]
    public float maxWalkVelocity;
    public float maxSprintVelocity;
    public float maxVelocity;
    public float minVelocity;
    #endregion

    #region Friction
    [Header("Friction Values")]
    public float friction;
    public float groundFriction;
    public float inAirFriction;
    public float slidingFriction;
    #endregion

    #region In Air
    [Header("In Air Variables")]
    [Range(0, 1)]
    public float inAirControl;
    public int inAirJumps;
    private int _inAirJumps;
    #endregion

    #region Gravity
    [Header("Gravity Variables")]
    public float initialGravity;
    public float gravityRate;
    public float maxGravity;
    public float jumpingInitialGravity;
    #endregion

    #region Jump
    [Header("Jump Variables")]
    public float jumpBuffer;
    float _jumpBuffer;
    public float jumpStrength;
    public float jumpStregthDecreaser;
    public float jumpInAirStrength;
    public float highestPointHoldTime;
    float _highestPointHoldTimer;
    public float justJumpedCooldown;
    float _justJumpedCooldown;
    public float coyoteTime;
    float _coyoteTimer;
    #endregion

    #region Crouch
    [Header("Crouch Variables")]
    public bool crouchBuffer;
    public bool topIsClear;
    #endregion

    #region Slide
    [Header("Slide Variables")]
    public float velocityToSlide;
    public float slideForce;
    [Range(0,1)]
    public float slideControl;
    #endregion

    #region Climbing Checks
    [Space]
    public bool feetSphereCheck;
    public bool kneesCheck;
    public float fakeGroundTime;
    float _fakeGroundTimer;

    public bool feetCheck;
    public bool headCheck;
    public bool forwardCheck;
    #endregion

    #region Vault
    [Header("Vault Variables")]
    public float vaultClimbStrength;
    #endregion

    #region Climb
    [Header("Climbing Variables")]
    public float negativeVelocityToClimb;
    public float climbingDuration;
    float _climbingTime;
    public float climbAcceleration;
    public float maxClimbingVelocity;
    public float initialClimbingGravity;
    float _climbingGravity;
    public float climbingGravityMultiplier;
    public float climbingStrafe;
    float _climbingStrafe;
    public float climbStrafeDecreaser;
    public float maxClimbStrafeVelocity;
    public float climbingStrafeFriction;
    public float endOfClimbJumpStrength;
    public float climbingCooldown;
    float _climbingCooldown;
    #endregion

    #region WallJump
    [Header("WallJump Variables")]
    public float wallJumpHeightStrenght;
    public float wallJumpNormalStrength;
    #endregion

    #region Vectors
    Vector3 groundedForward;
    Vector3 groundedRight;

    Vector3 totalVelocityToAdd;
    Vector3 newForwardandRight;
    Vector3 currentForwardAndRight;
    Vector3 velocityAtCollision;
    #endregion

    #region Raycast hits
    RaycastHit hit;
    RaycastHit forwardHit;
    RaycastHit feetHit;
    #endregion

     private WaitForFixedUpdate fixedUpdate;
    #endregion

    void Start()
    {
        capCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        moveCamera = GetComponent<MoveCamera>();
        fixedUpdate = new WaitForFixedUpdate();
        groundCheckDistance = capCollider.height * .5f - capCollider.radius;
        friction = inAirFriction;
        g = initialGravity;
        playerState = PlayerState.InAir;
        //Time.timeScale = .1f;
    }

    void Update()
    {
        crouchBuffer = Input.GetKey(KeyCode.LeftControl);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = (isCrouching) ? false : true;
        }
        speedIncrease = (isSprinting) ? sprintSpeedIncrease : walkSpeedIncrease;
        maxVelocity = (isSprinting && !isCrouching)? maxSprintVelocity : maxWalkVelocity;

        if (Input.GetKey(KeyCode.W)) z = speedIncrease;
        else if (Input.GetKey(KeyCode.S)) z = -speedIncrease;
        else  z = 0;                             
        if (Input.GetKey(KeyCode.D)) x = speedIncrease;
        else if (Input.GetKey(KeyCode.A)) x = -speedIncrease;
        else x = 0;

        scrollWheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKeyDown(KeyCode.Space) || scrollWheelDelta > 0)
        {
            _jumpBuffer = jumpBuffer;
        }
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Move();
        Crouch();
        HandleJumpInput();
        ApplyGravity();
        ClimbingChecks();
        HandleVault();
        rb.velocity += totalVelocityToAdd;
        if (rb.velocity.magnitude < minVelocity && x == 0 && z == 0 && (isGrounded))        //If the player stops moving set its maxVelocity to walkingSpeed and set its rb velocity to 0
        {
            rb.velocity = Vector3.zero;
            isSprinting = false;
        }
        //HandleClimb();
    }

    private void GroundCheck()
    {
        if(_coyoteTimer> 0)_coyoteTimer -= Time.fixedDeltaTime;
        if (_justJumpedCooldown > 0) _justJumpedCooldown -= Time.fixedDeltaTime;
        groundCheck = (_justJumpedCooldown <= 0) ? Physics.SphereCast(transform.position, capCollider.radius, -transform.up, out hit, groundCheckDistance + 0.01f) : false;
        slope = Vector3.Angle(hit.normal, Vector3.up);
        if ( slope > maxSlope)
        {
                groundCheck = false;
                previousState = playerState;
                playerState = PlayerState.InAir;
                g = initialGravity;           
        }
        totalVelocityToAdd = Vector3.zero;
        newForwardandRight = Vector3.zero;

        groundedForward = Vector3.Cross(hit.normal, -transform.right);
        groundedRight = Vector3.Cross(hit.normal, transform.forward);

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
        if (groundCheck && (playerState == PlayerState.Jumping || playerState == PlayerState.InAir || playerState == PlayerState.Climbing))
        {
            rb.velocity = rb.velocity - Vector3.up * rb.velocity.y;
            if (!onFakeGround && hit.normal.y != 1)rb.velocity = (groundedRight* x + groundedForward* z).normalized * rb.velocity.magnitude;          //This is to prevent the weird glitch where the player bounces on slopes if they land on them without jumping
            friction = groundFriction;
            _climbingCooldown = 0;
            previousState = playerState;
            playerState = PlayerState.Grounded;
            g = 0;
            _inAirJumps = inAirJumps;
        }
        if (isGrounded && !groundCheck)
        {
            if (playerState != PlayerState.Jumping)
            {
                previousState = playerState;
                playerState = PlayerState.InAir;
                g = initialGravity;
            }
            _coyoteTimer = coyoteTime;
            friction = inAirFriction;
        }
        isGrounded = groundCheck;
    }
    private void ClimbingChecks()
    {
        float maxDistance = capCollider.radius * (1 + ((isSprinting)?(rb.velocity.magnitude / maxSprintVelocity): 0) );
        if (playerState == PlayerState.Grounded) feetSphereCheck = Physics.SphereCast(transform.position - Vector3.up * .5f, capCollider.radius + .01f, rb.velocity.normalized, out feetHit, maxDistance);
        headCheck = Physics.Raycast(Camera.main.transform.position + Vector3.up * .25f, transform.forward, capCollider.radius + .1f);
        forwardCheck = (Physics.Raycast(transform.position, transform.forward, capCollider.radius + .1f));  //forwardCheck = Physics.Raycast(transform.position, transform.forward, capCollider.radius + ((slope >= 70? capCollider.radius:.1f)));
        if (forwardCheck && currentForwardAndRight.magnitude > 1) velocityAtCollision = currentForwardAndRight;        
        if (feetSphereCheck && !onFakeGround)
        {
            kneesCheck = Physics.Raycast(transform.position - Vector3.up * capCollider.height * .24f, transform.forward, maxDistance + capCollider.radius);
            if (!kneesCheck && playerState == PlayerState.Grounded && (x != 0 || z != 0))StartCoroutine(FakeGround());
        }
        kneesCheck = false;
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
                    rb.velocity = newForwardandRight.normalized * currentForwardAndRight.magnitude * inAirControl + currentForwardAndRight * (1f - inAirControl) + rb.velocity.y * Vector3.up;
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
            else if(playerState != PlayerState.Sliding)
            {
                if ((z == 0 && x == 0) || (pvX < 0 && x > 0) || (x < 0 && pvX > 0) || (pvZ < 0 && z > 0) || (z < 0 && pvZ > 0)) rb.velocity *= .99f; //Decrease 
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
    private void Crouch()
    {
        topIsClear = !Physics.Raycast(transform.position - newForwardandRight.normalized * capCollider.radius, Vector3.up, capCollider.height + .01f); // Check if thee's nothing blocking the player from standing up
        //Crouch
        if (!isCrouching && crouchBuffer)
        {
            capCollider.height *= .5f;
            capCollider.center += Vector3.up * -.5f;
            isCrouching = true;
            moveCamera.AdjustCameraHeight(true);
        }
        //Stand Up
        if (isCrouching && !crouchBuffer)
        {
            if (topIsClear) //Checks that there are no obstacles on top of the player so they can stand up
            {
                capCollider.height *= 2f;
                capCollider.center += Vector3.up * .5f;
                isCrouching = false;
                moveCamera.AdjustCameraHeight(false);
            }
        }
    }
    private void HandleJumpInput()
    {
        if(_jumpBuffer <= 0 ) _jumpBuffer = 0;
        if (playerState != PlayerState.Climbing)
        {
            if (_jumpBuffer > 0 && (isGrounded || _coyoteTimer > 0) && playerState!=PlayerState.Jumping && topIsClear) StartCoroutine(JumpCoroutine(false));
            else if (playerState == PlayerState.InAir && _inAirJumps > 0 && _jumpBuffer > 0)
            {
                _inAirJumps--;
                StartCoroutine(JumpCoroutine(true));
            }
        }
        if (_jumpBuffer > 0) _jumpBuffer -= Time.fixedDeltaTime;
    }
    private void ApplyGravity()
    {
        if (playerState != PlayerState.Climbing)
        {
            if (!isGrounded)
            {
                totalVelocityToAdd += Vector3.up * g;
            }
            if (g > maxGravity) g *= gravityRate;
        }
    }
    private void HandleVault()
    {
        if (playerState != PlayerState.Vaulting && forwardCheck && !headCheck && z > 0) StartCoroutine(VaultCoroutine());
    }
    private void HandleClimb()
    {
        if (_climbingCooldown > 0) _climbingCooldown -= Time.fixedDeltaTime;
        if (playerState == PlayerState.InAir && forwardCheck && rb.velocity.y > negativeVelocityToClimb && (z > 0 || currentForwardAndRight.magnitude > 0f) && _climbingCooldown <= 0)
            StartCoroutine(ClimbCoroutine());
    }
    private IEnumerator FakeGround()
    {
        onFakeGround = true;
        transform.position = new Vector3(transform.position.x, feetHit.point.y + 1f, transform.position.z);
        g = 0;
        _fakeGroundTimer = fakeGroundTime;
        while (_fakeGroundTimer > 0 && onFakeGround)
        {
            _fakeGroundTimer -= Time.fixedDeltaTime;
            yield return fixedUpdate;
        }
        onFakeGround = false;
    }
    private IEnumerator SlideCoroutine()
    {
        friction = slidingFriction;
        previousState = playerState;
        playerState = PlayerState.Sliding;
        totalVelocityToAdd += currentForwardAndRight * slideForce;
        maxVelocity = maxWalkVelocity;
        isSprinting = false;
        while (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = newForwardandRight.normalized * rb.velocity.magnitude * slideControl + rb.velocity * (1f - slideControl);
            if (!isGrounded)
            {
                friction = inAirFriction;
                previousState = PlayerState.Sliding;
                isSprinting = true;
                yield break;
            }
            if (!crouchBuffer)
            {
                if (rb.velocity.magnitude > maxWalkVelocity) isSprinting = true;
                previousState = playerState;
                playerState = PlayerState.Grounded;
                yield break;
            }
            yield return fixedUpdate;
        }
        friction = groundFriction;
        previousState = playerState;
        playerState = PlayerState.Grounded;
    }
    private IEnumerator JumpCoroutine(bool inAirJump)
    {
        //print("started");
        _jumpBuffer = 0;
        previousState = playerState;
        playerState = PlayerState.Jumping;
        y = jumpStrength;
        g = jumpingInitialGravity;
        _justJumpedCooldown = justJumpedCooldown;
        totalVelocityToAdd += newForwardandRight;
        if (inAirJump && (x!=0 || z!=0)) rb.velocity = newForwardandRight.normalized *  ((currentForwardAndRight.magnitude< maxSprintVelocity)? maxSprintVelocity:currentForwardAndRight.magnitude); 
        else rb.velocity -= rb.velocity.y * Vector3.up;
        while (rb.velocity.y >= 0f && playerState!=PlayerState.Grounded)
        {
            y -= jumpStregthDecreaser;
            totalVelocityToAdd += Vector3.up * y;
            yield return fixedUpdate;
        }
        if(playerState != PlayerState.Grounded)
        {
            _highestPointHoldTimer = highestPointHoldTime;
            g = 0;
            rb.velocity -= Vector3.up * rb.velocity.y;
            while (_highestPointHoldTimer > 0)
            {
                _highestPointHoldTimer -= Time.fixedDeltaTime; 
                yield return fixedUpdate;
            }
            g = initialGravity;
        }
        previousState = playerState;
        if(!isGrounded)playerState = PlayerState.InAir;
    }
    private IEnumerator VaultCoroutine()
    {
        previousState = playerState;
        playerState = PlayerState.Vaulting;
        rb.velocity = Vector3.up * vaultClimbStrength;
        float height = Camera.main.transform.position.y;
        Physics.BoxCast(transform.position - transform.forward.normalized * capCollider.radius * .5f, Vector3.one * capCollider.radius, transform.forward, out forwardHit, Quaternion.identity, 1f);
        feetCheck = (Physics.Raycast(transform.position - Vector3.up * capCollider.height * .5f, transform.forward, capCollider.radius + .1f));
        while ((transform.position.y - capCollider.height * .5)<height )
        {
            //feetCheck = (Physics.Raycast(transform.position - Vector3.up * capCollider.height * .5f, transform.forward, capCollider.radius + .1f));
            rb.velocity += Vector3.up * .05f;
            yield return fixedUpdate;
        }
        feetCheck = false;
        previousState = playerState;
        if (!isGrounded) playerState = PlayerState.InAir;
        rb.velocity = -forwardHit.normal * 10;

    }
    private IEnumerator ClimbCoroutine()
    {
        previousState = playerState;
        playerState = PlayerState.Climbing;
        _climbingTime = climbingDuration;
        _justJumpedCooldown = 0;
        _climbingGravity = initialClimbingGravity;
        Physics.BoxCast(transform.position - transform.forward.normalized * capCollider.radius * .5f, Vector3.one * capCollider.radius, transform.forward, out forwardHit, Quaternion.identity, 1f);
        _climbingStrafe = climbingStrafe;
        Vector3 playerOnWallRightDirection = Vector3.Cross(forwardHit.normal, Vector3.up).normalized;
        Vector3 originalHorizontalClimbingDirection = Vector3.Project(velocityAtCollision, playerOnWallRightDirection);
        rb.velocity = originalHorizontalClimbingDirection;
        while (!isGrounded && forwardCheck && playerState == PlayerState.Climbing && _climbingTime > 0)
        {
            if (_jumpBuffer > 0)
            {
                rb.velocity += Vector3.up * wallJumpHeightStrenght + forwardHit.normal * wallJumpNormalStrength;
                g = jumpingInitialGravity;
                _jumpBuffer = 0;
                _justJumpedCooldown = justJumpedCooldown;
                _climbingCooldown = climbingCooldown;
                previousState = playerState;
                playerState = PlayerState.InAir;
                yield break;
            }
            rb.velocity += Vector3.up * ((z > 0) ? (rb.velocity.y > maxClimbingVelocity ? 0 : climbAcceleration) : (originalHorizontalClimbingDirection.magnitude > 7.5f) ? 0 : -_climbingGravity);
            rb.velocity += (currentForwardAndRight.magnitude < maxClimbStrafeVelocity) ? playerOnWallRightDirection * x * _climbingStrafe : Vector3.zero - currentForwardAndRight * climbingStrafeFriction;
            _climbingGravity *= climbingGravityMultiplier;
            _climbingTime -= Time.fixedDeltaTime;
            _climbingStrafe -= climbStrafeDecreaser;

            yield return fixedUpdate;
        }
        if (playerState != PlayerState.Vaulting)
        {
            rb.velocity += Vector3.up + Vector3.Cross(forwardHit.normal, Vector3.up).normalized * x + forwardHit.normal * endOfClimbJumpStrength;
        }
        _climbingCooldown = climbingCooldown;
        previousState = playerState;
        if (!isGrounded) playerState = PlayerState.InAir;
        else rb.velocity = -Vector3.up * rb.velocity.y;
    }
}
