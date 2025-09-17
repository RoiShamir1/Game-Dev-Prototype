using UnityEngine;

public class Pistol : Weapon
{
    public override void Shoot()
    {
        base.Shoot();
        Debug.Log("Pistol fired!");
    }
}
