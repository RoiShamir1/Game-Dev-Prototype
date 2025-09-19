using UnityEngine;

public class SMG : Weapon
{
    float nextFireTime;

    public override void Shoot()
    {
        if (data == null || data.bulletPrefab == null || firePoint == null) return;
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + data.fireRate;

        for (int i = 0; i < Mathf.Max(1, data.bulletsPerShot); i++)
        {
            float angle = Random.Range(-data.spreadAngle, data.spreadAngle);
            var rot = firePoint.rotation * Quaternion.Euler(0, 0, angle);
            var b = Instantiate(data.bulletPrefab, firePoint.position, rot);
            var rb = b.GetComponent<Rigidbody2D>();
            if (rb) rb.linearVelocity = (Vector2)(rot * Vector3.right) * data.bulletForce;
        }

        OnWeaponFired?.Raise();
    }
}
