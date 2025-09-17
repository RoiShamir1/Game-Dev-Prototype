using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    public List<Weapon> weapons;
    private int currentWeaponIndex = 0;
    private Weapon currentWeapon;

    void Start()
    {
        if (weapons.Count > 0)
        {
            EquipWeapon(0);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && currentWeapon != null)
        {
            currentWeapon.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);
    }

    void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;

        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }

        weapons[index].gameObject.SetActive(true);
        currentWeapon = weapons[index];
        currentWeaponIndex = index;

        Debug.Log("Equipped: " + currentWeapon.name);
    }
}
