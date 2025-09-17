using UnityEngine;

public class SMG : Weapon
{
    public float fireRate = 0.1f;
    public int bulletsPerShot = 3;
    public float spreadAngle = 5f;

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }
    public override void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float angle = Random.Range(-spreadAngle, spreadAngle);
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, angle);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(rotation * Vector3.right * bulletForce, ForceMode2D.Impulse);
        }

        Debug.Log("SMG burst fired!");
    }
}
