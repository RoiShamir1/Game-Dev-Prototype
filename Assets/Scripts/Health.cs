using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    public bool destroyOnDeath = true;

    public int Current { get; private set; }

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    void Awake()
    {
        Current = maxHealth;
        Debug.Log($"[Health] {name}: Init HP {Current}/{maxHealth}");
    }

    public bool ApplyDamage(int dmg)
    {
        if (Current <= 0) return true;

        int amount = Mathf.Max(0, dmg);
        int before = Current;
        Current = Mathf.Max(0, Current - amount);

        Debug.Log($"[Health] {name}: -{amount} dmg, HP {Current}/{maxHealth} (was {before})");
        OnHealthChanged?.Invoke(Current, maxHealth);

        if (Current == 0)
        {
            Die();
            return true;
        }
        return false;
    }

    public void TakeDamage(int amount)
    {
        ApplyDamage(amount);
    }

    public void Kill()
    {
        if (Current <= 0) return;
        Current = 0;
        OnHealthChanged?.Invoke(Current, maxHealth);
        Die();
    }

    void Die()
    {
        Debug.Log($"[Health] {name}: DIED");
        OnDied?.Invoke();
        if (destroyOnDeath)
        {
            Destroy(gameObject);
            Debug.Log($"[Health] {name}: Destroyed");
        }
    }
}