using UnityEngine;
using UnityEngine.Events;
using AK.Wwise;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;

    public UnityEvent<int, int> onHealthChanged;
    public UnityEvent onDeath;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        onHealthChanged.AddListener((current, max) =>
            AkUnitySoundEngine.SetRTPCValue("PlayerHealthPara", max - current, gameObject));
    }

    public void Heal(int amount)
    {
        if (CurrentHealth <= 0) return;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        onHealthChanged.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (CurrentHealth <= 0) return;
        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        onHealthChanged.Invoke(CurrentHealth, maxHealth);
        if (CurrentHealth == 0)
            onDeath.Invoke();
    }
}
