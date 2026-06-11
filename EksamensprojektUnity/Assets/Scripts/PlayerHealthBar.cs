using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private PlayerHealth _playerHealth;

    private void Awake()
    {
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;
    }

    private void Start()
    {
        _playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        slider.maxValue = _playerHealth.MaxHealth;
        slider.value = _playerHealth.CurrentHealth;
        _playerHealth.onHealthChanged.AddListener(OnHealthChanged);
    }

    private void OnDestroy()
    {
        if (_playerHealth != null)
            _playerHealth.onHealthChanged.RemoveListener(OnHealthChanged);
    }

    private void OnHealthChanged(int current, int max)
    {
        slider.value = current;
    }
}
