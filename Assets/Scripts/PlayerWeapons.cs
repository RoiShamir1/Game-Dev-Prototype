using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapons : MonoBehaviour
{
    public List<Weapon> weapons;
    private int currentWeaponIndex = 0;
    private Weapon currentWeapon;

    void Start()
    {
        if (weapons == null || weapons.Count == 0)
        {
            Debug.LogWarning("No weapons assigned to PlayerWeapons.");
            return;
        }
        EquipWeapon(0);
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.isPressed && currentWeapon != null)
        {
            currentWeapon.Shoot();
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) EquipWeapon(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) EquipWeapon(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) EquipWeapon(2);
        }
    }

    void EquipWeapon(int index)
    {
        if (weapons == null || index < 0 || index >= weapons.Count) return;

        for (int i = 0; i < weapons.Count; i++)
        {
            var go = weapons[i]?.gameObject;
            if (go != null) go.SetActive(i == index);
        }

        currentWeapon = weapons[index];
        currentWeaponIndex = index;
        Debug.Log("Equipped: " + (currentWeapon ? currentWeapon.name : "NULL"));
    }
}