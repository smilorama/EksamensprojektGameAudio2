using UnityEngine;

public class ScaleByProximity : MonoBehaviour
{
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private float _fullScaleDistance = 10f;
    [SerializeField] private float _minScaleDistance = 1f;
    [SerializeField] private float _minScale = 0f;
    [SerializeField] private float _maxScale = 1f;
    [SerializeField] private bool _preserveOriginalScale = false;

    private Transform _player;
    private Vector3 _originalScale;

    private void Start()
    {
        _originalScale = _preserveOriginalScale ? transform.localScale : transform.localScale.normalized;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
    }

    private void Update()
    {
        if (_player == null || _targetPoint == null) return;

        float dist = Vector3.Distance(_player.position, _targetPoint.position);
        float t = Mathf.InverseLerp(_minScaleDistance, _fullScaleDistance, dist);
        float scale = Mathf.Lerp(_minScale, _maxScale, t);
        transform.localScale = _originalScale * scale;
    }
}
