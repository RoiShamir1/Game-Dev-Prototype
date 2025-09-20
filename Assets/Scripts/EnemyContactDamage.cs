using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    public enum Mode { Hits, TimeInContact }
    public Mode mode = Mode.Hits;

    [Header("General")]
    public string playerTag = "Player";

    [Header("Hits mode")]
    public int damagePerHit = 1;
    public float hitCooldown = 0.5f;

    [Header("TimeInContact mode")]
    public float killAfterSeconds = 3f;

    private float lastHitTime = -999f;
    private float contactTime = 0f;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (mode == Mode.Hits)
        {
            TryHit(other.GetComponent<Health>());
        }

        contactTime = 0f;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (mode == Mode.TimeInContact)
        {
            contactTime += Time.deltaTime;
            if (contactTime >= killAfterSeconds)
            {
                var h = other.GetComponent<Health>();
                if (h) h.Kill();
                contactTime = 0f;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) contactTime = 0f;
    }

    private void TryHit(Health h)
    {
        if (h == null) return;
        if (Time.time - lastHitTime < hitCooldown) return;
        lastHitTime = Time.time;
        h.TakeDamage(damagePerHit);
    }
}