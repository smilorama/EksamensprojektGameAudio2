using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

// Hierarchy:
//   Camera
//     WeaponHolder          <- this script
//       HandMesh            <- assign to HandMesh field
//         DamageZoneChild   <- PlayerDamageZone + trigger collider + kinematic Rigidbody

[System.Serializable]
public struct AttackKeyframe
{
    public Vector3 position;
    public Vector3 rotation;
}

public class WeaponBob : MonoBehaviour
{
    // ── References ───────────────────────────────────────────────────────────
    [Header("References")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private StarterAssetsInputs _input;
    [SerializeField] private PlayerDamageZone _damageZone;
    [SerializeField] private Transform _handMesh;

    // ── Bob ──────────────────────────────────────────────────────────────────
    [Header("Bob")]
    [SerializeField] private float _bobFrequency = 2.0f;
    [SerializeField] private float _bobAmplitudeY = 0.05f;
    [SerializeField] private float _bobAmplitudeX = 0.025f;
    [SerializeField] private float _walkThreshold = 0.1f;

    // ── Idle Drift ───────────────────────────────────────────────────────────
    [Header("Idle Drift")]
    [SerializeField] private float _idleDriftFrequency = 0.8f;
    [SerializeField] private float _idleDriftAmplitude = 0.008f;

    // ── Sway ─────────────────────────────────────────────────────────────────
    [Header("Sway")]
    [SerializeField] private float _swayAmount = 0.04f;
    [SerializeField] private float _swaySmoothing = 8.0f;
    [SerializeField] private float _swayMaxOffset = 0.06f;

    // ── Attack timing ─────────────────────────────────────────────────────────
    [Header("Attack — Timing")]
    [SerializeField] private float _attackCooldown = 0.8f;
    [Tooltip("Speed KF0 → KF1 (windup, keep low)")]
    [SerializeField] private float _windupSpeed = 4.0f;
    [Tooltip("Speed KF1 → KF2")]
    [SerializeField] private float _strikeSpeed0 = 12.0f;
    [Tooltip("Speed KF2 → KF3 (fastest — whip peak)")]
    [SerializeField] private float _strikeSpeed1 = 22.0f;
    [Tooltip("Speed KF3 → KF4 (follow-through, slows)")]
    [SerializeField] private float _strikeSpeed2 = 8.0f;
    [Tooltip("Speed KF4 → KF0 (recovery)")]
    [SerializeField] private float _recoverySpeed = 6.0f;
    [Tooltip("Normalised time [0-1] within KF2→KF3 segment when damage activates")]
    [SerializeField] private float _damageActivateAt = 0.5f;

    // ── Attack keyframes (Inspector-editable) ─────────────────────────────────
    [Header("Attack — Keyframes (offsets from HandMesh rest)")]
    [SerializeField] private AttackKeyframe _kf0 = new AttackKeyframe {
        position = new Vector3( 0f,      0f,      0f     ),
        rotation = new Vector3( 0f,      0f,      0f     )
    };
    [SerializeField] private AttackKeyframe _kf1 = new AttackKeyframe {
        position = new Vector3( 0.707f, -0.281f, -0.115f ),
        rotation = new Vector3(-23.571f,-28.96f, -21.73f )
    };
    [SerializeField] private AttackKeyframe _kf2 = new AttackKeyframe {
        position = new Vector3( 0.64f,  -0.0794f, 0.366f ),
        rotation = new Vector3(-9.312f, -41.499f,-25.606f)
    };
    [SerializeField] private AttackKeyframe _kf3 = new AttackKeyframe {
        position = new Vector3(-0.4f,   -0.2f,    0.8f   ),
        rotation = new Vector3(66.282f, -88.255f,-60.589f)
    };
    [SerializeField] private AttackKeyframe _kf4 = new AttackKeyframe {
        position = new Vector3(-0.8f,   -0.5f,    0.9f   ),
        rotation = new Vector3(62.201f,-151.221f,-122.53f)
    };

    // ── State ─────────────────────────────────────────────────────────────────
    private Vector3 _holderRestPosition;
    private Vector3 _handRestLocalPosition;
    private Quaternion _handRestLocalRotation;

    private enum AttackSegment { Idle, Windup, Strike0, Strike1, Strike2, Recovery }
    private AttackSegment _segment = AttackSegment.Idle;
    private float _segmentT;
    private bool _damageActivated;
    private float _attackCooldownTimer;

    private float _bobTimer;
    private float _idleTimer;
    private Vector2 _swayCurrentOffset;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Start()
    {
        _holderRestPosition = transform.localPosition;

        if (_handMesh != null)
        {
            _handRestLocalPosition = _handMesh.localPosition;
            _handRestLocalRotation = _handMesh.localRotation;
        }

        if (_characterController == null)
            _characterController = GetComponentInParent<CharacterController>();
        if (_input == null)
            _input = GetComponentInParent<StarterAssetsInputs>();
    }

    private void Update()
    {
        _attackCooldownTimer -= Time.deltaTime;

        if (Mouse.current.leftButton.wasPressedThisFrame && _attackCooldownTimer <= 0f && _segment == AttackSegment.Idle && !DialogueUI.IsActive)
            BeginAttack();

        TickAttack();

        transform.localPosition = _holderRestPosition + ComputeBob() + ComputeSway();
    }

    // ── Bob ──────────────────────────────────────────────────────────────────

    private Vector3 ComputeBob()
    {
        float speed = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z).magnitude;

        if (speed > _walkThreshold)
        {
            _idleTimer = 0f;
            _bobTimer += Time.deltaTime * _bobFrequency * (speed / 4f);
            float x = Mathf.Sin(_bobTimer) * _bobAmplitudeX;
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

    // ── Sway ─────────────────────────────────────────────────────────────────

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

    // ── Attack ────────────────────────────────────────────────────────────────

    private void BeginAttack()
    {
        _segment = AttackSegment.Windup;
        _segmentT = 0f;
        _damageActivated = false;
        _attackCooldownTimer = _attackCooldown;
    }

    private void TickAttack()
    {
        if (_segment == AttackSegment.Idle)
        {
            if (_handMesh != null)
            {
                _handMesh.localPosition = Vector3.Lerp(_handMesh.localPosition, _handRestLocalPosition, Time.deltaTime * _recoverySpeed);
                _handMesh.localRotation = Quaternion.Lerp(_handMesh.localRotation, _handRestLocalRotation, Time.deltaTime * _recoverySpeed);
            }
            return;
        }

        float speed = _segment switch {
            AttackSegment.Windup   => _windupSpeed,
            AttackSegment.Strike0  => _strikeSpeed0,
            AttackSegment.Strike1  => _strikeSpeed1,
            AttackSegment.Strike2  => _strikeSpeed2,
            AttackSegment.Recovery => _recoverySpeed,
            _                      => 1f,
        };

        _segmentT += Time.deltaTime * speed;
        float t = Mathf.Clamp01(_segmentT);

        switch (_segment)
        {
            case AttackSegment.Windup:
                ApplyPose(_kf0, _kf1, t);
                if (_segmentT >= 1f) NextSegment(AttackSegment.Strike0);
                break;

            case AttackSegment.Strike0:
                ApplyPose(_kf1, _kf2, t);
                if (_segmentT >= 1f) NextSegment(AttackSegment.Strike1);
                break;

            case AttackSegment.Strike1:
                ApplyPose(_kf2, _kf3, t);
                if (!_damageActivated && t >= _damageActivateAt)
                {
                    _damageZone?.Activate();
                    _damageActivated = true;
                }
                if (_segmentT >= 1f) NextSegment(AttackSegment.Strike2);
                break;

            case AttackSegment.Strike2:
                ApplyPose(_kf3, _kf4, t);
                if (_segmentT >= 1f)
                {
                    _damageZone?.Deactivate();
                    NextSegment(AttackSegment.Recovery);
                }
                break;

            case AttackSegment.Recovery:
                ApplyPose(_kf4, _kf0, t);
                if (_segmentT >= 1f) NextSegment(AttackSegment.Idle);
                break;
        }
    }

    private void NextSegment(AttackSegment next)
    {
        _segment  = next;
        _segmentT = 0f;
    }

    private void ApplyPose(AttackKeyframe from, AttackKeyframe to, float t)
    {
        if (_handMesh == null) return;
        float tEased = Mathf.SmoothStep(0f, 1f, t);
        _handMesh.localPosition = _handRestLocalPosition + Vector3.Lerp(from.position, to.position, tEased);
        _handMesh.localRotation = _handRestLocalRotation * Quaternion.Euler(Vector3.Lerp(from.rotation, to.rotation, tEased));
    }
}
