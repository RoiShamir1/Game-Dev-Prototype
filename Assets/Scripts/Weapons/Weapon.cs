using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData data;
    public Transform firePoint;
    public VoidEventChannelSO OnWeaponFired;

    public virtual void Shoot()
    {
        if (data == null || data.bulletPrefab == null || firePoint == null) return;

        var bullet = Instantiate(data.bulletPrefab, firePoint.position, firePoint.rotation);
        var rb = bullet.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = (Vector2)firePoint.right * data.bulletForce;

        OnWeaponFired?.Raise();
    }
}
