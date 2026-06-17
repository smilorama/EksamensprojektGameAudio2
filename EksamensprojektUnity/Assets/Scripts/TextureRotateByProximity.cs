using UnityEngine;

public class TextureRotateByProximity : MonoBehaviour
{
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private float _fullSpeedDistance = 1f;
    [SerializeField] private float _noSpeedDistance = 10f;
    [SerializeField] private float _maxRotationSpeed = 90f;
    [SerializeField] private Vector2 _scrollDirection = Vector2.right;

    private static readonly string RtpcName = "Rotate_Parameter";
    private static TextureRotateByProximity _closestInstance;

    private Transform _player;
    private Renderer _renderer;
    private float _lastDist = -1f;
    private float _currentDist = float.MaxValue;
    private float _lastT = -1f;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
    }

    private void Update()
    {
        if (_player == null || _targetPoint == null || _renderer == null) return;
        if (DialogueUI.Instance != null && DialogueUI.Instance.HasFlag("Tome Burned")) return;

        _currentDist = Vector3.Distance(_player.position, _targetPoint.position);
        float distDelta = _currentDist - _lastDist;
        _lastDist = _currentDist;

        float t = Mathf.InverseLerp(_fullSpeedDistance, _noSpeedDistance, _currentDist);
        float move = distDelta * _maxRotationSpeed * (1f - t);

        Vector2 currentOffset = _renderer.material.GetTextureOffset("_MainTex");
        Vector2 newOffset = currentOffset + _scrollDirection.normalized * move;
        _renderer.material.SetTextureOffset("_MainTex", newOffset);
        _renderer.material.SetTextureOffset("_BaseMap", newOffset);

        if (_closestInstance == null || _currentDist <= _closestInstance._currentDist)
            _closestInstance = this;

        if (_closestInstance == this && t != _lastT)
        {
            AkUnitySoundEngine.SetRTPCValue(RtpcName, (1f - t) * 100f);
            _lastT = t;
        }
    }

    private void OnDisable()
    {
        if (_closestInstance == this) _closestInstance = null;
    }
}
