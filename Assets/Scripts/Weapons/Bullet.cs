using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float lifeTime = 3f;
    public int damage = 1;

    [Header("Events (optional)")]
    public VoidEventChannelSO OnEnemyHit;
    public VoidEventChannelSO OnEnemyDied;

    [Header("Debug")]
    public bool enableDebug = true;
    public bool destroyOnNonEnemyTrigger = false; // אם true - הורס גם כשהכדור פוגע בטריגר לא-אויב

    private int enemyLayer = -1;
    private Rigidbody2D rb;

    void Awake()
    {
        enemyLayer = LayerMask.NameToLayer("Enemy");
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // חשוב לכדורים מהירים
    }

    void OnEnable()
    {
        if (enableDebug)
        {
            string myLayer = LayerMask.LayerToName(gameObject.layer);
            Debug.Log($"[Bullet] Spawned: {name}, layer={myLayer}, pos={transform.position}");
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ProcessHit(other, "OnTriggerEnter2D");

        // ברירת מחדל: כמו שהיה אצלך - לא הורס אם זה לא אויב
        if (destroyOnNonEnemyTrigger == false) return;

        // אם רוצים להרוס בכל מקרה גם כשזה לא אויב:
        if (enemyLayer == -1 || other.gameObject.layer != enemyLayer)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        ProcessHit(col.collider, "OnCollisionEnter2D");
        Destroy(gameObject);
    }

    private void ProcessHit(Collider2D col, string via)
    {
        if (col == null)
        {
            if (enableDebug) Debug.LogWarning("[Bullet] ProcessHit: collider is null");
            return;
        }

        string otherLayerName = LayerMask.LayerToName(col.gameObject.layer);
        if (enableDebug)
        {
            string myLayer = LayerMask.LayerToName(gameObject.layer);
            string tag = col.tag;
            bool isTrig = col.isTrigger;
            string rbInfo = rb ? $" vel={rb.linearVelocity} mode={rb.collisionDetectionMode}" : " rb=null";
            Debug.Log($"[Bullet] {via} -> hit {col.name} (layer={otherLayerName}, tag={tag}, isTrigger={isTrig}) | bullet(layer={myLayer}){rbInfo}");
        }

        if (enemyLayer == -1)
        {
            if (enableDebug) Debug.LogError("[Bullet] Enemy layer does not exist. Create a layer named 'Enemy' and assign it to the enemy colliders.");
            return;
        }

        if (col.gameObject.layer != enemyLayer)
        {
            if (enableDebug) Debug.Log("[Bullet] Hit non-enemy collider. No damage applied.");
            return;
        }

        // מצא Health גם אם הוא על ההורה (קוליידר יכול להיות על ילד)
        var health = col.GetComponentInParent<Health>();
        if (health == null)
        {
            if (enableDebug) Debug.LogWarning($"[Bullet] Enemy layer hit but no Health found on parent of {col.name}.");
            return;
        }

        bool died = false;
        try
        {
            died = health.ApplyDamage(damage);
            if (enableDebug) Debug.Log($"[Bullet] Applied {damage} damage to {health.name}. died={died}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Bullet] Exception while applying damage to {health.name}: {ex}");
        }

        if (died)
        {
            if (enableDebug) Debug.Log("[Bullet] Raising OnEnemyDied event.");
            OnEnemyDied?.Raise();
        }
        else
        {
            if (enableDebug) Debug.Log("[Bullet] Raising OnEnemyHit event.");
            OnEnemyHit?.Raise();
        }

        Destroy(gameObject);
    }
}