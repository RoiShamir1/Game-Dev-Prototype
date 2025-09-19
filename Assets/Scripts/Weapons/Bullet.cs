using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 3f;
    public string targetTag = "Enemy";

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")
            || other.GetComponentInParent<EnemyFollow>() != null)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        //Destroy(gameObject);
        TryDamage(c.collider.gameObject);
    }


    private void TryDamage(GameObject other)
    {
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
        {
            Destroy(gameObject);
            return;
        }

        var health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
