using UnityEngine;

public class ScaleByProximity : MonoBehaviour
{
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private float _fullScaleDistance = 10f;
    [SerializeField] private float _minScaleDistance = 1f;
    [SerializeField] private float _minScale = 0f;
    [SerializeField] private float _maxScale = 1f;
    [SerializeField] private bool _preserveOriginalScale = false;

    private static readonly string RtpcName = "Scaling_Parameter";
    private static ScaleByProximity _closestInstance;

    private Transform _player;
    private Vector3 _originalScale;
    private float _currentDist = float.MaxValue;
    private float _lastScale = -1f;

    private void Start()
    {
        _originalScale = _preserveOriginalScale ? transform.localScale : transform.localScale.normalized;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
    }

    private void Update()
    {
        if (_player == null || _targetPoint == null) return;
        if (DialogueUI.Instance != null && DialogueUI.Instance.HasFlag("Tome Burned")) return;

        _currentDist = Vector3.Distance(_player.position, _targetPoint.position);
        float t = Mathf.InverseLerp(_minScaleDistance, _fullScaleDistance, _currentDist);
        float scale = Mathf.Lerp(_minScale, _maxScale, t);
        transform.localScale = _originalScale * scale;

        // Find closest instance this frame
        if (_closestInstance == null || _currentDist <= _closestInstance._currentDist)
            _closestInstance = this;

        float proximity = 1f - Mathf.InverseLerp(_minScaleDistance, _fullScaleDistance, _currentDist);
        if (_closestInstance == this && proximity != _lastScale)
        {
            AkUnitySoundEngine.SetRTPCValue(RtpcName, proximity * 100f);
            _lastScale = proximity;
        }
    }

    private void OnDisable()
    {
        if (_closestInstance == this) _closestInstance = null;
    }
}
