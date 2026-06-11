using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    public enum ItemType { Consumable, EventTrigger }

    [SerializeField] private ItemType itemType;
    [SerializeField] private int healAmount = 25;
    public UnityEvent onPickup;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (itemType == ItemType.Consumable)
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.Heal(healAmount);
        }
        else
        {
            onPickup.Invoke();
        }

        Destroy(gameObject);
    }
}
