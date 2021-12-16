using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    #region Variables

    #region Movement Mechanics
    [Header("Additional Mechanics")]
    public bool jumpMechanic;
    public bool crouchMechanic;
    public bool vaultMechanic;
    #endregion

    #region Additional Mechanics Variables
    public BaseMovementVariables baseMovementVariables = new BaseMovementVariables();
    public CrouchVariables crouchVariables = new CrouchVariables();
    public SlideVariables slideVariables = new SlideVariables();
    public JumpVariables jumpVariables = new JumpVariables();
    public VaultVariables vaultVariables = new VaultVariables();
    public ClimbVariables climbVariables = new ClimbVariables();
    #endregion

    #region Player States
    [Header("Player States")]
    public bool isGrounded;
    bool groundCheck;
    public bool isSprinting;
    public bool onFakeGround;
    public PlayerState playerState;
    public PlayerState previousState;
    #endregion

    #region Primitive Variables
    private float x, z;
    private float g;
    private float pvX, pvZ;
    private float y;
    #endregion

    #region Global Variables

    #region Basic Movement
    private float surfaceSlope;
    private float maxVelocity;
    private float speedIncrease;
    private float friction;
    private float airControl;
    #endregion

    #region Jump
    private float _jumpBuffer;
    private float _highestPointHoldTimer;
    private float _justJumpedCooldown;
    private float _coyoteTimer;
    public int _inAirJumps;
    #endregion

    #region InAirVariables
    private float distanceToGround;
    private float timeSinceGrounded;
    #endregion

    #endregion

    #region Vectors
    Vector3 groundedForward;
    Vector3 groundedRight;

    Vector3 totalVelocityToAdd;
    Vector3 newForwardandRight;
    Vector3 currentForwardAndRight;
    Vector3 velocityAtCollision;

    Vector3 lastViablePosition;
    #endregion

    #region Raycast hits
    [HideInInspector] public RaycastHit hit;
    [HideInInspector] public RaycastHit feetHit;
    [HideInInspector] public RaycastHit forwardHit;
    [HideInInspector] public RaycastHit rayToGround;
    #endregion

    #region Events and delegates
    public delegate void PlayerBecameGrounded();
    public event PlayerBecameGrounded playerJustLanded;
    public delegate void PlayerLeftTheGround();
    public event PlayerLeftTheGround playerLeftGround;
    #endregion

    #region Components
    Rigidbody rb;
    CapsuleCollider capCollider;
    public MoveCamera playerCamera;
    #endregion

    #region Other
    private WaitForFixedUpdate fixedUpdate;
    public static PlayerController singleton;
    #endregion

    #endregion
    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        capCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        //moveCamera = GetComponent<MoveCamera>();
        fixedUpdate = new WaitForFixedUpdate();
        friction = baseMovementVariables.inAirFriction;
        airControl = baseMovementVariables.inAirControl;
        g = baseMovementVariables.initialGravity;
        playerState = PlayerState.InAir;
        baseMovementVariables.StartVariables(capCollider);
    }

    void Update()
    {
        if(crouchMechanic)CrouchInput();
        MovementInput();
        if(jumpMechanic)JumpInput();
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Move();
        if (crouchMechanic) HandleCrouchInput();
        if (jumpMechanic) HandleJumpInput();
        ApplyGravity();
        rb.velocity += totalVelocityToAdd;
        if (rb.velocity.magnitude < baseMovementVariables.minVelocity && x == 0 && z == 0 && (isGrounded))        //If the player stops moving set its maxVelocity to walkingSpeed and set its rb velocity to 0
        {
            rb.velocity = Vector3.zero;
            isSprinting = false;
        }
        if (vaultMechanic) ClimbChecks();
    }
}
