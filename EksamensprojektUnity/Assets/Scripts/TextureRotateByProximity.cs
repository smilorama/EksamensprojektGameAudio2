using UnityEngine;

public class TextureRotateByProximity : MonoBehaviour
{
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private float _fullSpeedDistance = 1f;
    [SerializeField] private float _noSpeedDistance = 10f;
    [SerializeField] private float _maxRotationSpeed = 90f;
    [SerializeField] private Vector2 _scrollDirection = Vector2.right;

    private Transform _player;
    private Renderer _renderer;
    private float _lastDist = -1f;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
    }

    private void Update()
    {
        if (_player == null || _targetPoint == null || _renderer == null) return;

        float dist = Vector3.Distance(_player.position, _targetPoint.position);
        float distDelta = dist - _lastDist;
        _lastDist = dist;

        float t = Mathf.InverseLerp(_fullSpeedDistance, _noSpeedDistance, dist);
        float move = distDelta * _maxRotationSpeed * (1f - t);

        Vector2 currentOffset = _renderer.material.GetTextureOffset("_MainTex");
        Vector2 newOffset = currentOffset + _scrollDirection.normalized * move;
        _renderer.material.SetTextureOffset("_MainTex", newOffset);
        _renderer.material.SetTextureOffset("_BaseMap", newOffset);
    }
}
