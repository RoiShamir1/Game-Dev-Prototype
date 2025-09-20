using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float fireRate = 1f;   
    public float bulletSpeed = 10f;
    public float spawnOffset = 0.2f; 

    float lastShotTime;
    bool playerInRange = false;
    Transform player;

    void Update()
    {
        if (playerInRange && player && Time.time > lastShotTime + 1f / fireRate)
        {
            Debug.Log("Enemy shoots"); 
            ShootAtPlayer();
            lastShotTime = Time.time;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    void ShootAtPlayer()
    {
        if (player == null || firePoint == null || bulletPrefab == null) return;

       
        Vector2 dir = ((Vector2)player.position - (Vector2)firePoint.position).normalized;

       
        Vector2 spawnPos = (Vector2)firePoint.position + dir * spawnOffset;

        
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        
        var rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.freezeRotation = true;
            rb.linearVelocity = dir * bulletSpeed;
        }

        
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, ang);

       
        var bulletCol = bullet.GetComponent<Collider2D>();
        if (bulletCol != null)
        {
            var enemyCols = GetComponentsInChildren<Collider2D>();
            foreach (var ec in enemyCols)
                Physics2D.IgnoreCollision(bulletCol, ec, true);
        }
    }

    public void SetTarget(Transform t)
    {
        playerInRange = true;
        player = t;
    }

    public void ClearTarget(Transform t)
    {
        if (player == t)
        {
            playerInRange = false;
            player = null;
        }
    }

}
