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

    [Header("Player X Deadzone")]
    [SerializeField] float stopDistanceX = 0.30f;

    [Header("Stop when player is not alive/active")]
    [SerializeField] bool stopWhenPlayerNotAlive = true;

    [Header("Thresholds & Animation")]
    [SerializeField] float minCommandEpsilon = 0.01f;  
    [SerializeField] float movingThreshold = 0.05f;    
    [SerializeField] float animDamp = 0.10f;   

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
        if (!player || (stopWhenPlayerNotAlive && !IsPlayerAlive()))
        {
            ApplyVelocity(0f);
            UpdateFacingAndAnimation(0f);
            return;
        }

        float dx = player.position.x - transform.position.x;

        if (Mathf.Abs(dx) < stopDistanceX)
        {
            ApplyVelocity(0f);
            UpdateFacingAndAnimation(0f);
            return;
        }

        int sign = dx > 0f ? 1 : -1;

        bool wallAhead = false;
        if (wallCheck)
        {
            Vector2 dir = new(sign, 0f);
            var hit = Physics2D.Raycast(wallCheck.position, dir, wallCheckDistance, groundWalls);
            if (hit.collider != null && Mathf.Abs(hit.normal.x) > wallNormalThreshold)
                wallAhead = true;
        }

        float vxCmd = wallAhead ? 0f : sign * speed;

        ApplyVelocity(vxCmd);
        UpdateFacingAndAnimation(vxCmd);
    }

    bool IsPlayerAlive()
    {
        if (!player || !player.gameObject.activeInHierarchy) return false;

        var pm = player.GetComponent<PlayerMovement>();
        if (pm && !pm.enabled) return false;

        return true;
    }

    public void OnPlayerDied()
    {
        ApplyVelocity(0f);
        UpdateFacingAndAnimation(0f);
        enabled = true;
    }

    void ApplyVelocity(float move)
    {
        if (rb && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);
        }
        else
        {
            transform.position += new Vector3(move * Time.fixedDeltaTime, 0f, 0f);
        }
    }

    void UpdateFacingAndAnimation(float move)
    {
        if (Mathf.Abs(move) > minCommandEpsilon)
        {
            var s = transform.localScale;
            s.x = Mathf.Abs(baseScale.x) * (move >= 0f ? 1f : -1f);
            transform.localScale = s;
        }

        float actual = rb && rb.bodyType == RigidbodyType2D.Dynamic
            ? rb.linearVelocity.x
            : (transform.position.x - lastX) / Time.fixedDeltaTime;

        if (animator)
        {
            animator.SetFloat("Speed", Mathf.Abs(actual), animDamp, Time.fixedDeltaTime);
            animator.SetBool("isMoving", Mathf.Abs(actual) > movingThreshold);
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
