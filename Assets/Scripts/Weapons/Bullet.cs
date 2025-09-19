using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 1;
    public VoidEventChannelSO OnEnemyHit;
    public VoidEventChannelSO OnEnemyDied;

    void Start() { Destroy(gameObject, lifeTime); }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                bool died = health.ApplyDamage(damage);
                if (died) OnEnemyDied?.Raise();
                else OnEnemyHit?.Raise();
            }
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D _) { Destroy(gameObject); }
}
