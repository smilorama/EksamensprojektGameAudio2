using UnityEngine;
using StarterAssets;

public class PlayerJumpAudio : MonoBehaviour
{
    [SerializeField] private string _jumpEvent = "";
    [SerializeField] private string _landEvent = "";

    private FirstPersonController _fpc;
    private bool _wasGrounded = true;

    private void Start()
    {
        _fpc = GetComponent<FirstPersonController>();
    }

    private void Update()
    {
        if (_fpc == null) return;

        bool grounded = _fpc.Grounded;

        if (_wasGrounded && !grounded && !string.IsNullOrEmpty(_jumpEvent))
            AkUnitySoundEngine.PostEvent(_jumpEvent, gameObject);

        if (!_wasGrounded && grounded && !string.IsNullOrEmpty(_landEvent))
            AkUnitySoundEngine.PostEvent(_landEvent, gameObject);

        _wasGrounded = grounded;
    }
}
