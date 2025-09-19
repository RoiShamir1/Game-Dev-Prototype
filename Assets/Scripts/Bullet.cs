using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // פגיעה בקירות / רצפה
    void OnCollisionEnter2D(Collision2D c)
    {
        Destroy(gameObject);
    }

    // פגיעה באויב
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")
            || other.GetComponentInParent<EnemyFollow>() != null)
        {
            // כאן תוכל גם לקרוא לפונקציה של האויב (למשל לקחת חיים)
            Destroy(gameObject);
        }
    }
}
