using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 2f;

    [Header("Wall Stop")]
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundWalls;
    [SerializeField] float wallCheckDistance = 0.08f;
    [SerializeField] float wallNormalThreshold = 0.7f;

    [Header("X Deadzone (to prevent jitter when player is above)")]
    [SerializeField] float stopDistanceX = 0.30f; // אם |dx| קטן מזה — לא זזים בכלל

    Transform player;
    Vector3 baseScale;
    Rigidbody2D rb;
    Animator animator;
    float lastX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        baseScale = transform.localScale;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void FixedUpdate()
    {
        if (!player) return;

        float dx = player.position.x - transform.position.x;
        float dy = player.position.y - transform.position.y;

        // --- Deadzone בציר X: אם השחקן כמעט בדיוק מעלינו, לא מחליפים כיוון/לא זזים ---
        if (Mathf.Abs(dx) < stopDistanceX)
        {
            ApplyVelocity(0f);
            UpdateFacingAndAnimation(0f);
            return;
        }

        // קבע כיוון (ימין/שמאל) לפי dx
        int sign = dx > 0f ? 1 : -1;

        // עצירה מול קיר/מדרגה באמצעות Raycast מה-wallCheck
        bool wallAhead = false;
        if (wallCheck)
        {
            Vector2 dir = new(sign, 0f);
            var hit = Physics2D.Raycast(wallCheck.position, dir, wallCheckDistance, groundWalls);
            if (hit.collider != null && Mathf.Abs(hit.normal.x) > wallNormalThreshold)
                wallAhead = true;
        }

        float vxCmd = wallAhead ? 0f : sign * speed;

        // תנועה בפועל
        ApplyVelocity(vxCmd);

        // הפניית ספרייט + אנימציה
        UpdateFacingAndAnimation(vxCmd);
    }

    // === Helpers ===
    void ApplyVelocity(float vxCmd)
    {
        if (rb && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.linearVelocity = new Vector2(vxCmd, rb.linearVelocity.y);
        }
        else
        {
            transform.position += new Vector3(vxCmd * Time.fixedDeltaTime, 0f, 0f);
        }
    }

    void UpdateFacingAndAnimation(float vxCmd)
    {
        // הפניית הספרייט לפי כיוון התנועה
        if (Mathf.Abs(vxCmd) > 0.01f)
        {
            var s = transform.localScale;
            s.x = Mathf.Abs(baseScale.x) * (vxCmd >= 0f ? 1f : -1f);
            transform.localScale = s;
        }

        // מהירות בפועל (לצרכי אנימציה)
        float vxActual = rb && rb.bodyType == RigidbodyType2D.Dynamic
            ? rb.linearVelocity.x
            : (transform.position.x - lastX) / Time.fixedDeltaTime;

        if (animator)
        {
            animator.SetFloat("Speed", Mathf.Abs(vxActual), 0.1f, Time.fixedDeltaTime);
            animator.SetBool("isMoving", Mathf.Abs(vxActual) > 0.05f);
        }

        lastX = transform.position.x;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (wallCheck)
        {
            Gizmos.color = Color.cyan;
            float sign = Mathf.Sign(transform.localScale.x);
            Gizmos.DrawLine(
                wallCheck.position,
                wallCheck.position + Vector3.right * sign * wallCheckDistance
            );
        }
    }
#endif
}
