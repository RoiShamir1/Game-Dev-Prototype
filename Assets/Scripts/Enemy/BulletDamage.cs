using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private string playerTag = "Player";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DealDamage(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DealDamage(other);
    }

    private void DealDamage(Collider2D col)
    {
        if (!col.CompareTag(playerTag)) return;

        // לחפש את קומפוננטת ה-Health על השחקן
        var health = col.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log($"[BulletDamage] {name} hit {col.name}, damage={damage}");
        }

        // הכדור נעלם אחרי פגיעה
        Destroy(gameObject);
    }
}
