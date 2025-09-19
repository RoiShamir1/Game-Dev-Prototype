using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 10f;

    public virtual void Shoot()
    {
        Debug.Log("Bang! Shooting from: " + gameObject.name);

        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning($"{name}: bulletPrefab or firePoint not assigned.");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = (Vector2)firePoint.right * bulletForce;
        }
        else
        {
            Debug.LogWarning($"{name}: Bullet prefab missing Rigidbody2D.");
        }
    }
}