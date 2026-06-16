using UnityEngine;
using StarterAssets;

public class OffHandBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponBob _mainHand;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private StarterAssetsInputs _input;

    [Header("Bob")]
    [SerializeField] private float _bobFrequency = 2.0f;
    [SerializeField] private float _bobAmplitudeY = 0.04f;
    [SerializeField] private float _bobAmplitudeX = 0.02f;
    [SerializeField] private float _walkThreshold = 0.1f;

    [Header("Idle Drift")]
    [SerializeField] private float _idleDriftFrequency = 0.8f;
    [SerializeField] private float _idleDriftAmplitude = 0.006f;

    [Header("Sway")]
    [SerializeField] private float _swayAmount = 0.03f;
    [SerializeField] private float _swaySmoothing = 8.0f;
    [SerializeField] private float _swayMaxOffset = 0.05f;

    private Vector3 _restPosition;
    private float _bobTimer;
    private float _idleTimer;
    private Vector2 _swayCurrentOffset;

    private void Start()
    {
        _restPosition = transform.localPosition;

        if (_characterController == null)
            _characterController = GetComponentInParent<CharacterController>();
        if (_input == null)
            _input = GetComponentInParent<StarterAssetsInputs>();
    }

    private void Update()
    {
        transform.localPosition = _restPosition + ComputeBob() + ComputeSway();
    }

    private Vector3 ComputeBob()
    {
        float speed = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z).magnitude;

        if (speed > _walkThreshold)
        {
            _idleTimer = 0f;
            _bobTimer += Time.deltaTime * _bobFrequency * (speed / 4f);
            float x = -Mathf.Sin(_bobTimer) * _bobAmplitudeX;
            float y = -Mathf.Abs(Mathf.Sin(_bobTimer)) * _bobAmplitudeY;
            return new Vector3(x, y, 0f);
        }
        else
        {
            _bobTimer = 0f;
            _idleTimer += Time.deltaTime;
            float y = Mathf.Sin(_idleTimer * _idleDriftFrequency * Mathf.PI * 2f) * _idleDriftAmplitude;
            return new Vector3(0f, y, 0f);
        }
    }

    private Vector3 ComputeSway()
    {
        Vector2 look = _input.look;
        Vector2 target = new Vector2(
            Mathf.Clamp(-look.x * _swayAmount, -_swayMaxOffset, _swayMaxOffset),
            Mathf.Clamp(-look.y * _swayAmount, -_swayMaxOffset, _swayMaxOffset)
        );
        _swayCurrentOffset = Vector2.Lerp(_swayCurrentOffset, target, Time.deltaTime * _swaySmoothing);
        return new Vector3(_swayCurrentOffset.x, _swayCurrentOffset.y, 0f);
    }
}
