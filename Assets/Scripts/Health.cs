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
    }

    public bool ApplyDamage(int dmg)
    {
        maxHealth = Mathf.Max(0, maxHealth - Mathf.Max(0, dmg));
        return maxHealth == 0;
    }

    public void TakeDamage(int amount)
    {
        if (Current <= 0) return;
        Current -= amount;
        OnHealthChanged?.Invoke(Current, maxHealth);
        if (Current <= 0) Die();
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
        OnDied?.Invoke();
        if (destroyOnDeath) Destroy(gameObject);
    }
}