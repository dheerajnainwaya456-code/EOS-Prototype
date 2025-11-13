using System.Collections;
using NUnit.Framework.Internal;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Component references
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    // States and state machine
    public PlayerInputSet input { get; private set; }
    private StateMachine stateMachine;
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_ClimbingState climbingState { get; private set; }

    // Attack Variables
    [Header("Attack Details")]
    public Vector2[] attackVelocity;
    public float attackVelocityDuration = 0.1f;
    public float comboResetTime = 2;
    private Coroutine queuedAttackCo;

    // Movement Variables
    public Vector2 moveInput { get; private set; }

    [Header("Movement Details")]
    public float moveSpeed;

    // Jump Variables
    public float jumpForce = 30;
    public int inAirCounter = 5;
    public Vector2 wallJumpForce;
    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;

    public float jumpBufferTime = 0.15f;
    public float jumpBufferTimer = 0f;

    public bool jumpBuffered => jumpBufferTimer > 0;

    // Climbing Variables
    public float climbingSpeed = 10;

    [Range(0, 1)]
    public float inAirMultiplier = 0.7f;
    [Range(0, 1)]
    public float wallSlideMultiplier = 0.3f;
    [Space]
    public float dashDuration = 0.25f;
    public float dashSpeed = 20;

    [Header("Collision Detection")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatisGround;
    [SerializeField] private LayerMask whatisStaris;
    public bool groundDetacted { get; private set; }
    public bool stairsDetacted;
    public bool wallDetacted { get; private set; }


    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();
        input = new PlayerInputSet();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        climbingState = new Player_ClimbingState(this, stateMachine, "climb");
    }

    void OnEnable()
    {
        input.Enable();

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Start()
    {
        stateMachine.Initialize(idleState);
    }

    void Update()
    {

        if (input.Player.Jump.WasPressedThisFrame())
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        HandleConllisionDetaction();
        stateMachine.UpdateActiveState();
    }

    public void ConsumeJumpBuffer()
    {
        jumpBufferTimer = 0f;
    }

    public void EnterAttackState()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    public void setVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && !facingRight)
        {
            Flip();
        }
        if (xVelocity < 0 && facingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {
        transform.Rotate(0, 180, 0);

        facingRight = !facingRight;
        facingDir = facingDir * (-1);
    }

    private void HandleConllisionDetaction()
    {
        groundDetacted = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatisGround);
        stairsDetacted = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, whatisStaris);
        wallDetacted = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatisGround);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(wallCheckDistance * facingDir, 0, 0));
    }
}