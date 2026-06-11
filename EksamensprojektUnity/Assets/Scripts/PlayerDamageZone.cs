using UnityEngine;

// Attach to a child of WeaponHolder (trigger collider + kinematic Rigidbody).
// WeaponBob calls Activate() at the peak of the swing and Deactivate() on recovery.
// Hits each Enemy once per swing — resets when Activate() is called again.

public class PlayerDamageZone : MonoBehaviour
{
    [SerializeField] private int damage = 15;

    private bool _active;

    public void Activate()
    {
        _active = true;
    }

    public void Deactivate()
    {
        _active = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_active) return;
        if (!other.CompareTag("Enemy")) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
            enemy.TakeDamage(damage);

        _active = false;
    }
}
