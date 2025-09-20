using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private const float DIRECTION_DEADZONE = 0.01f;
    private const float ENEMY_TOP_NORMAL_Y = 0.70f;
    private const float MIN_FIRE_RATE = 0.05f;
    private const float FALLBACK_FIRE_RATE = 0.25f;

    Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] float sprintMod = 1.5f;
    [SerializeField] float speed = 10f;
    [SerializeField] float jumpSpeed = 12f;

    bool isGrounded = true;
    float directionX;

    Animator animator;
    SpriteRenderer sprite;
    InputSystem inputActions;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.15f;
    [SerializeField] LayerMask groundLayer = ~0;

    [Header("Enemy Standing (Coyote Time)")]
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float onEnemyCoyoteTime = 0.12f;
    float onEnemyTopTimer;

    [Header("Weapons")]
    [SerializeField] Transform weaponHolder;
    [SerializeField] List<Weapon> weapons = new();
    [SerializeField] int startWeaponIndex = 0;

    int currentWeaponIndex = -1;
    Weapon currentWeapon;
    bool isFiring;
    float nextShotTime;

    void Awake()
    {
        inputActions = new InputSystem();
        if (!weaponHolder) weaponHolder = transform.Find("WeaponHolder");
    }

    void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Sprint.started += OnSprintStarted;
        inputActions.Player.Sprint.canceled += OnSprintCanceled;

        inputActions.Player.Fire.started += OnFireStarted;
        inputActions.Player.Fire.canceled += OnFireCanceled;

        inputActions.Player.Next.performed += OnNextWeaponPerformed;
        inputActions.Player.Previous.performed += OnPrevWeaponPerformed;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        if (weapons == null || weapons.Count == 0)
            weapons = GetComponentsInChildren<Weapon>(true).ToList();

        for (int i = 0; i < weapons.Count; i++)
            if (weapons[i]) weapons[i].gameObject.SetActive(false);

        EquipWeapon(Mathf.Clamp(startWeaponIndex, 0, Mathf.Max(0, weapons.Count - 1)));
    }

    void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Sprint.started -= OnSprintStarted;
        inputActions.Player.Sprint.canceled -= OnSprintCanceled;

        inputActions.Player.Fire.started -= OnFireStarted;
        inputActions.Player.Fire.canceled -= OnFireCanceled;

        inputActions.Player.Next.performed -= OnNextWeaponPerformed;
        inputActions.Player.Previous.performed -= OnPrevWeaponPerformed;

        inputActions.Player.Disable();
    }

    void OnMovePerformed(InputAction.CallbackContext ctx) => directionX = ctx.ReadValue<Vector2>().x;
    void OnMoveCanceled(InputAction.CallbackContext ctx) => directionX = 0f;

    void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (!(IsGroundedCircle() || onEnemyTopTimer > 0f)) return;
        HandleJump();
        onEnemyTopTimer = 0f;
    }

    void OnSprintStarted(InputAction.CallbackContext ctx) => speed *= sprintMod;
    void OnSprintCanceled(InputAction.CallbackContext ctx) => speed /= sprintMod;

    void OnFireStarted(InputAction.CallbackContext _) { isFiring = true; TryShoot(); }
    void OnFireCanceled(InputAction.CallbackContext _) { isFiring = false; }

    void OnNextWeaponPerformed(InputAction.CallbackContext _) => NextWeapon();
    void OnPrevWeaponPerformed(InputAction.CallbackContext _) => PrevWeapon();

    void Update()
    {
        isGrounded = IsGroundedCircle();

        if (onEnemyTopTimer > 0f)
            onEnemyTopTimer -= Time.deltaTime;

        if (Mathf.Abs(directionX) > DIRECTION_DEADZONE)
        {
            bool facingLeft = directionX < 0f;
            if (sprite) sprite.flipX = facingLeft;
            if (weaponHolder)
                weaponHolder.localRotation = facingLeft ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }

        if (animator)
        {
            if (rb) animator.SetFloat("Speed", rb.linearVelocity.x);
            animator.SetBool("isGrounded", isGrounded || onEnemyTopTimer > 0f);
        }

        if (isFiring && currentWeapon && currentWeapon.data && currentWeapon.data.autoFire)
            TryShoot();
    }

    void FixedUpdate()
    {
        if (!rb) return;
        var v = rb.linearVelocity;
        v.x = directionX * speed;
        rb.linearVelocity = v;
    }

    void HandleJump()
    {
        isGrounded = false;
        if (animator) animator.SetTrigger("IsJumping");
        if (rb) rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
    }

    bool IsGroundedCircle()
    {
        if (!groundCheck) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
#endif

    void EquipWeapon(int index)
    {
        if (weapons == null || weapons.Count == 0) { currentWeaponIndex = -1; currentWeapon = null; return; }
        if (index < 0 || index >= weapons.Count) return;

        if (currentWeaponIndex >= 0 && currentWeaponIndex < weapons.Count && weapons[currentWeaponIndex])
            weapons[currentWeaponIndex].gameObject.SetActive(false);

        currentWeaponIndex = index;
        currentWeapon = weapons[currentWeaponIndex];
        if (currentWeapon) currentWeapon.gameObject.SetActive(true);

        nextShotTime = 0f;
    }

    void NextWeapon()
    {
        if (weapons == null || weapons.Count <= 1) return;
        EquipWeapon((currentWeaponIndex + 1) % weapons.Count);
    }
    void PrevWeapon()
    {
        if (weapons == null || weapons.Count <= 1) return;
        EquipWeapon((currentWeaponIndex - 1 + weapons.Count) % weapons.Count);
    }

    void TryShoot()
    {
        if (!currentWeapon) return;

        float rate = (currentWeapon.data != null)
            ? Mathf.Max(MIN_FIRE_RATE, currentWeapon.data.fireRate)
            : FALLBACK_FIRE_RATE;

        if (Time.time < nextShotTime) return;

        currentWeapon.Shoot();
        nextShotTime = Time.time + rate;
    }

    void OnCollisionStay2D(Collision2D c)
    {
        int lm = 1 << c.collider.gameObject.layer;
        if ((enemyLayer.value & lm) == 0) return;

        foreach (var ct in c.contacts)
        {
            if (ct.normal.y > ENEMY_TOP_NORMAL_Y)
            {
                onEnemyTopTimer = onEnemyCoyoteTime;
                break;
            }
        }
    }
}
