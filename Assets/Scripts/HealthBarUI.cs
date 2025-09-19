using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Slider slider;

    void Awake()
    {
        if (health == null)
            health = Object.FindFirstObjectByType<Health>();
    }

    void OnEnable()
    {
        if (health != null && slider != null)
        {
            slider.maxValue = health.maxHealth;
            slider.value = health.Current;
            health.OnHealthChanged += OnHealthChanged;
        }
    }

    void OnDisable()
    {
        if (health != null)
            health.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int current, int max)
    {
        if (slider != null)
        {
            slider.maxValue = max;
            slider.value = current;
        }
    }
}