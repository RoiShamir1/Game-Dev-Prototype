using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] float speed = 2f;

    [Header("Detect")]
    [SerializeField] Transform wallCheck;        // גרור לפה את ה-child
    [SerializeField] LayerMask groundWalls;      // הרצפה/קירות
    [SerializeField] float wallCheckDistance = 0.06f; // כמה קדימה לבדוק
    [SerializeField] float wallNormalThreshold = 0.7f; // כמה אנכי נחשב "קיר"

    Rigidbody2D rb;
    Transform player;
    Vector3 baseScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseScale = transform.localScale;
        if (rb) rb.freezeRotation = true;
    }

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void FixedUpdate()
    {
        if (!player) return;

        // כיוון אופקי לכיוון השחקן
        float sign = Mathf.Sign(player.position.x - transform.position.x);
        Vector2 dir = new Vector2(sign, 0f);

        // בדיקת "קיר" מול הפנים
        bool wallAhead = false;
        if (wallCheck)
        {
            var hit = Physics2D.Raycast(wallCheck.position, dir, wallCheckDistance, groundWalls);
            if (hit.collider != null && Mathf.Abs(hit.normal.x) > wallNormalThreshold)
                wallAhead = true; // פגיעה כמעט-אנכית = קיר/בליטה
        }

        float vx = wallAhead ? 0f : sign * speed;

        if (rb && rb.bodyType == RigidbodyType2D.Dynamic)
            rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
        else
            transform.position += new Vector3(vx * Time.fixedDeltaTime, 0f, 0f);

        // הפיכת הספרייט לכיוון התנועה
        var s = transform.localScale;
        s.x = Mathf.Abs(baseScale.x) * (sign >= 0 ? 1f : -1f);
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (wallCheck)
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * Mathf.Sign(transform.localScale.x) * wallCheckDistance);
    }
}
