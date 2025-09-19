using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 2f;

    [Header("Wall Stop")]
    [SerializeField] Transform wallCheck;          // גרור לכאן את ה-child
    [SerializeField] LayerMask groundWalls;        // סמן שכבות של קיר/רצפה
    [SerializeField] float wallCheckDistance = 0.08f;
    [SerializeField] float wallNormalThreshold = 0.7f;

    Transform player;
    Vector3 baseScale;
    Rigidbody2D rb;

    // Animator + מדידת מהירות בפועל
    Animator animator;
    float lastX;

    void Awake()
    {
        baseScale = transform.localScale;

        rb = GetComponent<Rigidbody2D>();
        if (rb) rb.freezeRotation = true;

        animator = GetComponent<Animator>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        lastX = transform.position.x;
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void FixedUpdate()
    {
        if (!player) return;

        // כיוון אופקי אל השחקן
        float sign = Mathf.Sign(player.position.x - transform.position.x);
        Vector2 dir = new Vector2(sign, 0f);

        // בדיקת "קיר מול הפנים"
        bool wallAhead = false;
        if (wallCheck)
        {
            var hit = Physics2D.Raycast(wallCheck.position, dir, wallCheckDistance, groundWalls);
            if (hit.collider != null && Mathf.Abs(hit.normal.x) > wallNormalThreshold)
                wallAhead = true;
        }

        // תנועה/עצירה
        float vxCmd = wallAhead ? 0f : sign * speed;

        if (rb && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            // Rigidbody2D משתמשים ב-velocity (לא linearVelocity)
            rb.linearVelocity = new Vector2(vxCmd, rb.linearVelocity.y);
        }
        else
        {
            transform.position += new Vector3(vxCmd * Time.fixedDeltaTime, 0f, 0f);
        }

        // הפיכת ספרייט לפי הכיוון
        var s = transform.localScale;
        s.x = Mathf.Abs(baseScale.x) * (sign >= 0 ? 1f : -1f);
        transform.localScale = s;

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

    void OnDrawGizmosSelected()
    {
        if (wallCheck)
        {
            float sign = Mathf.Sign(transform.localScale.x);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * sign * wallCheckDistance);
        }
    }
}
