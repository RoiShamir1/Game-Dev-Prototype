using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer; 
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float sprintMod = 1.5f;

    [Header("Components")]
    [SerializeField] private Transform flashlight;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private InputSystem inputActions;
    private float directionX;
    private bool isGrounded; 

    private void Awake()
    {
        inputActions = new();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ValidateComponent(rb, nameof(rb));
        animator = GetComponent<Animator>();
        ValidateComponent(animator, nameof(animator));
        sprite = GetComponent<SpriteRenderer>();
        ValidateComponent(sprite, nameof(sprite));
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Sprint.started += OnSprintStarted;
        inputActions.Player.Sprint.canceled += OnSprintCanceled;
    }

    void Update()
    {
        HandleMovementAnimations();
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
        HandleMovement();
        Debug.Log("Is the player grounded? " + isGrounded);
    }

    private void CheckIfGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //animator.SetBool("IsGrounded", isGrounded);
    }

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(directionX * speed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (isGrounded)
        {
            animator.SetTrigger("IsJumping");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }
    }

    private void HandleMovementAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(directionX * speed));

        if (directionX != 0)
        {
            bool faceLeft = directionX < 0;
            sprite.flipX = faceLeft;
            if (flashlight != null)
                flashlight.localRotation = Quaternion.Euler(0f, faceLeft ? 180f : 0f, 0f);
        }
    }

    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        HandleJump();
    }

    private void OnSprintCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => speed /= sprintMod;
    private void OnSprintStarted(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => speed *= sprintMod;
    private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => directionX = 0;
    private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => directionX = ctx.ReadValue<Vector2>().x;

    private void ValidateComponent<T>(T component, string name)
    {
        if (component is null) Debug.LogError($"{name} is null!");
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Sprint.started -= OnSprintStarted;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}