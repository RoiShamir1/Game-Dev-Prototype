using UnityEngine;

[CreateAssetMenu(menuName = "Data/Weapon Data", fileName = "New Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public float bulletForce = 18f;

    [Header("Auto/Burst")]
    [Min(0.01f)] public float fireRate = 0.2f;   
    [Min(1)] public int bulletsPerShot = 1;    
    [Range(0f, 45f)] public float spreadAngle = 0f;

    [Header("Mode")]
    public bool autoFire = false;               
}
