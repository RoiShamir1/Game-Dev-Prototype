using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    public enum Mode { Hits, TimeInContact }
    public Mode mode = Mode.Hits;

    public string playerTag = "Player";

    [Header("Hits mode")]
    public int damagePerHit = 1;
    public float hitCooldown = 0.6f;

    [Header("TimeInContact mode")]
    public float killAfterSeconds = 3f;

    private float lastHitTime = -999f;
    private float contactTime = 0f;

    void OnTriggerEnter2D(Collider2D other)  { HandleEnter(other); }
    void OnTriggerStay2D(Collider2D other)   { HandleStay(other); }
    void OnTriggerExit2D(Collider2D other)   { HandleExit(other); }

    void OnCollisionEnter2D(Collision2D c)   { HandleEnter(c.collider); }
    void OnCollisionStay2D(Collision2D c)    { HandleStay(c.collider); }
    void OnCollisionExit2D(Collision2D c)    { HandleExit(c.collider); }

    void HandleEnter(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        contactTime = 0f;
        if (mode == Mode.Hits) TryHit(other);
    }

    void HandleStay(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (mode == Mode.Hits)
        {
            TryHit(other);
        }
        else
        {
            var h = other.GetComponentInParent<Health>();
            if (h == null) return;

            contactTime += Time.deltaTime;

            float step = killAfterSeconds / Mathf.Max(1, h.maxHealth);
            if (contactTime >= step)
            {
                contactTime = 0f;
                bool died = h.ApplyDamage(1);
                if (died) contactTime = 0f;
            }
        }
    }

    void HandleExit(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        contactTime = 0f;
    }

    void TryHit(Collider2D other)
    {
        var h = other.GetComponentInParent<Health>();
        if (h == null) return;
        if (Time.time - lastHitTime < hitCooldown) return;

        lastHitTime = Time.time;
        h.ApplyDamage(damagePerHit);
    }
}