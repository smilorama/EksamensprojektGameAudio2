using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    private bool _active;

    public void Activate() => _active = true;
    public void activate() => _active = true;

    public void Deactivate() => _active = false;
    public void deactivate() => _active = false;

    private void OnTriggerEnter(Collider other) => TryHit(other);
    private void OnTriggerStay(Collider other) => TryHit(other);

    private void TryHit(Collider other)
    {
        if (!_active) return;
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
            health.TakeDamage(damage);

        _active = false;
    }
}
