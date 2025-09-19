using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 2f;
    [SerializeField] float jumpVelocity = 5f;

    [Header("Checks")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.12f;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] Transform wallCheck;
    [SerializeField] float wallDetectDistance = 0.4f;
    [SerializeField] LayerMask wallLayer;

    [Header("Animation")]
    [SerializeField] Animator animator;

    [Header("Debug")]
    [SerializeField] bool showDebugLogs = false;
    [SerializeField] bool drawGizmos = true;

    [SerializeField] float jumpCooldown = 0.2f;
    float nextJumpTime = 0f;

    Rigidbody2D rb;
    Transform player;
    Vector3 baseScale;

    private int moveIntent = 0; // -1 = שמאלה, 1 = ימינה

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseScale = transform.localScale;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {
        if (!player) return;

        // קביעת כיוון רצוי (intent)
        moveIntent = player.position.x < transform.position.x ? -1 : 1;

        // סיבוב האויב לפי כיוון
        transform.localScale = new Vector3(Mathf.Abs(baseScale.x) * moveIntent, baseScale.y, baseScale.z);
    }

    void FixedUpdate()
    {
        if (!player) return;

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        Vector2 origin = wallCheck != null ? (Vector2)wallCheck.position : (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * moveIntent, wallDetectDistance, wallLayer);
        bool isWall = hit.collider != null;

        // תנועה מול קיר
        float targetVelocityX = !isWall ? moveIntent * speed : 0f;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);

        // קפיצה מעל קיר קטן
        if (isWall && isGrounded && Time.time >= nextJumpTime)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            nextJumpTime = Time.time + jumpCooldown;

            if (animator != null)
                animator.SetTrigger("IsJumping");
        }

        // עדכון אנימציה לפי כוונה ללכת (moveIntent) ולא לפי מהירות פיזית
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveIntent));
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            float dir = transform.localScale.x < 0 ? -1f : 1f;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, 0.1f);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * dir * wallDetectDistance);
        }
    }
}
