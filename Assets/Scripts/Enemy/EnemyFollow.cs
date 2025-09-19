using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 2f;
    [SerializeField] float jumpVelocity = 5f;

    [Header("Wall Stop")]
    [SerializeField] Transform wallCheck;          // גרור לכאן את ה-child
    [SerializeField] LayerMask groundWalls;        // סמן שכבות של קיר/רצפה
    [SerializeField] float wallCheckDistance = 0.08f;
    [SerializeField] float wallNormalThreshold = 0.7f;
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

    Transform player;
    Vector3 baseScale;
    Rigidbody2D rb;

    // Animator + מדידת מהירות בפועל
    Animator animator;
    float lastX;

    private int moveIntent = 0; // -1 = שמאלה, 1 = ימינה

    void Awake()
    {
        baseScale = transform.localScale;

        rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

        animator = GetComponent<Animator>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        lastX = transform.position.x;
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

        // כיוון אופקי אל השחקן

        // בדיקת "קיר מול הפנים"
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
                wallAhead = true;
            if (animator != null)
                animator.SetTrigger("IsJumping");
        }

        // תנועה/עצירה
        float vxCmd = wallAhead ? 0f : sign * speed;
        {
            animator.SetFloat("Speed", Mathf.Abs(moveIntent));
        }
    }

    void OnDrawGizmosSelected()
        {
            // Rigidbody2D משתמשים ב-velocity (לא linearVelocity)
            rb.linearVelocity = new Vector2(vxCmd, rb.linearVelocity.y);
        }
        {
            transform.position += new Vector3(vxCmd * Time.fixedDeltaTime, 0f, 0f);
        }

        // הפיכת ספרייט לפי הכיוון
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // --- עדכון Animator לפי מהירות אמיתית אחרי התנועה ---
        float vxActual = (rb && rb.bodyType == RigidbodyType2D.Dynamic)
                         ? rb.linearVelocity.x
                         : (transform.position.x - lastX) / Time.fixedDeltaTime;
        lastX = transform.position.x;

        // "נדליק" הליכה מיד כשפקדנו לזוז, גם אם המהירות עוד לא נמדדה בפריים הזה
        bool movingCmd = Mathf.Abs(vxCmd) > 0.01f;
        bool movingNow = Mathf.Abs(vxActual) > 0.05f;
        bool isMoving = movingCmd || movingNow;

        if (animator)
        {
            animator.SetFloat("Speed", Mathf.Abs(vxActual), 0.1f, Time.fixedDeltaTime); // החלקה קלה
            animator.SetBool("isMoving", isMoving);
        }
        }

        if (wallCheck != null)
        {
            float dir = transform.localScale.x < 0 ? -1f : 1f;
        {
            float sign = Mathf.Sign(transform.localScale.x);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * sign * wallCheckDistance);
        }
    }
}
