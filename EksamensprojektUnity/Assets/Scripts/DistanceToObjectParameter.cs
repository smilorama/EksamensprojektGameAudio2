using UnityEngine;

public class DistanceToObjectParameter : MonoBehaviour
{
    [SerializeField] private string _rtpcName = "";
    [SerializeField] private float _maxDistance = 40f;

    private Transform _player;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
    }

    private void Update()
    {
        if (_player == null || string.IsNullOrEmpty(_rtpcName)) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        float value = (1f - Mathf.Clamp01(dist / _maxDistance)) * 100f;
        AkUnitySoundEngine.SetRTPCValue(_rtpcName, value);
    }
}
