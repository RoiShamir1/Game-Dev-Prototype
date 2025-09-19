using UnityEngine;

public class SMG : Weapon
{
    public float fireRate = 0.1f;
    public int bulletsPerShot = 3;
    public float spreadAngle = 5f;

    private float nextFireTime = 0f;

    public override void Shoot()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("SMG missing bulletPrefab or firePoint!");
            return;
        }

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float angle = Random.Range(-spreadAngle, spreadAngle);
            Quaternion rot = firePoint.rotation * Quaternion.Euler(0, 0, angle);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rot);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = (Vector2)(rot * Vector3.right) * bulletForce;
            }
        }

        Debug.Log("SMG burst fired!");
    }
}