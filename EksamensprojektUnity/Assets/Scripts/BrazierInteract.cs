using UnityEngine;

public class BrazierInteract : MonoBehaviour
{
    [Header("Fire Effect")]
    [SerializeField] private bool _startLit = false;
    [SerializeField] private Transform _fireSpawnPoint;
    [SerializeField] private GameObject _fireEffectPrefab;
    [SerializeField] private Vector3 _spawnOffset = Vector3.up;
    [SerializeField] private Vector3 _spawnRotation = Vector3.zero;
    [SerializeField] private Vector3 _spawnScale = Vector3.one;

    [Header("Emission")]
    [SerializeField] private float _emissionIntensity = 2f;
    [SerializeField] private Color _emissionColor = new Color(1f, 0.5f, 0.1f);

    private GameObject _spawnedFire;

    private void Start()
    {
        if (_startLit)
            SpawnFire();
    }

    public void SpawnFire()
    {
        if (_spawnedFire != null || _fireEffectPrefab == null) return;

        Vector3 spawnPos = _fireSpawnPoint != null ? _fireSpawnPoint.position : transform.TransformPoint(_spawnOffset);
        Quaternion rotation = (_fireSpawnPoint != null ? _fireSpawnPoint.rotation : transform.rotation) * Quaternion.Euler(_spawnRotation);
        _spawnedFire = Instantiate(_fireEffectPrefab, spawnPos, rotation);
        _spawnedFire.transform.localScale = _spawnScale;

        foreach (var ps in _spawnedFire.GetComponentsInChildren<ParticleSystem>())
            ps.Play();

        foreach (var r in _spawnedFire.GetComponentsInChildren<ParticleSystemRenderer>())
        {
            if (r.sharedMaterial == null) continue;
            Material mat = new Material(r.sharedMaterial);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", _emissionColor * _emissionIntensity);
            r.material = mat;
        }
    }

    public void ExtinguishFire()
    {
        if (_spawnedFire != null)
        {
            Destroy(_spawnedFire);
            _spawnedFire = null;
        }
    }
}
