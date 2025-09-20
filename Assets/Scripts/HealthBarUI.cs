using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Slider slider;

    void Awake()
    {
        if (slider == null)
            slider = GetComponentInChildren<Slider>(true);

        if (health == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player) health = player.GetComponent<Health>();
            if (health == null) health = Object.FindFirstObjectByType<Health>();
        }       
    }

    void OnEnable()
    {
        if (health == null || slider == null) return;

        health.OnHealthChanged += OnHealthChanged;
        health.OnDied += OnDied;

        InitializeBar();
    }

    void Start()
    {
        InitializeBar();
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.OnHealthChanged -= OnHealthChanged;
            health.OnDied -= OnDied;
        }
    }

    private void InitializeBar()
    {
        if (health == null || slider == null) return;

        slider.minValue = 0;
        slider.wholeNumbers = true;
        slider.maxValue = health.maxHealth;

        slider.SetValueWithoutNotify(health.Current);

        Canvas.ForceUpdateCanvases();
    }

    private void OnHealthChanged(int current, int max)
    {
        slider.maxValue = max;
        slider.SetValueWithoutNotify(current);
    }

    private void OnDied()
    {
        slider.SetValueWithoutNotify(0);
    }
}