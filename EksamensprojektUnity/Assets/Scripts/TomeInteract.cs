using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class TomeInteract : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private VolumeProfile _postTomeProfile;
    [SerializeField] private GameObject _leftHandWithTome;
    [SerializeField] private GameObject _preTomeLevelDesign;
    [SerializeField] private GameObject _postTomeLevelDesign;
    [SerializeField] private Renderer _tomeRenderer;

    [Header("Emission")]
    [SerializeField] private float _emissionStartIntensity = 0f;
    [SerializeField] private float _emissionIntensity = 3f;
    [SerializeField] private float _emissionFadeDuration = 1.5f;
    [SerializeField] private float _emissionPeakIntensity = 6f;
    [SerializeField] private float _emissionPeakFadeDuration = 0.8f;

    [Header("Book Rise")]
    [SerializeField] private float _riseHeight = 1.2f;
    [SerializeField] private float _riseSpeed = 0.8f;
    [SerializeField] private float _riseHoldDuration = 5f;

    [Header("Float (while hovering)")]
    [SerializeField] private float _rotateSpeed = 90f;
    [SerializeField] private float _bobAmplitude = 0.06f;
    [SerializeField] private float _bobFrequency = 1.2f;

    [Header("Timings")]
    [SerializeField] private float _handActivateDelay = 1.8f;
    [SerializeField] private float _levelSwapDelay = 2.2f;

    [Header("Audio")]
    [SerializeField] private string _voicelineEvent = "Play_TomePickup_Voice";
    [SerializeField] private GameObject _audioEmitter;

    [Header("Interact")]
    [SerializeField] private float _interactRange = 2.5f;
    [SerializeField] private float _promptRange = 2f;
    [SerializeField] private GameObject _interactPromptPanel;

    private Transform _player;
    private bool _triggered;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;

        if (_leftHandWithTome != null) _leftHandWithTome.SetActive(false);
        if (_postTomeLevelDesign != null) _postTomeLevelDesign.SetActive(false);
        if (_interactPromptPanel != null) _interactPromptPanel.SetActive(false);
    }

    private void SetEmissionIntensity(float intensity)
    {
        if (_tomeRenderer == null) return;
        Material mat = _tomeRenderer.material;
        mat.EnableKeyword("_EMISSION");
        Color baseEmission = mat.GetColor("_EmissionColor").linear;
        float h, s, v;
        Color.RGBToHSV(baseEmission, out h, out s, out v);
        Color hdrColor = Color.HSVToRGB(h, s, 1f, true) * intensity;
        mat.SetColor("_EmissionColor", hdrColor);
    }

    private IEnumerator FadeEmission(float from, float to, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            SetEmissionIntensity(Mathf.Lerp(from, to, t));
            yield return null;
        }
        SetEmissionIntensity(to);
    }

    private void Update()
    {
        if (_triggered || _player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        bool inPromptRange = dist <= _promptRange;

        if (_interactPromptPanel != null && _interactPromptPanel.activeSelf != inPromptRange)
            _interactPromptPanel.SetActive(inPromptRange);

        if (inPromptRange && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (_interactPromptPanel != null) _interactPromptPanel.SetActive(false);
            StartCoroutine(FadeEmission(_emissionStartIntensity, _emissionIntensity, _emissionFadeDuration));
            StartCoroutine(TomeSequence());
        }
    }

    private IEnumerator TomeSequence()
    {
        _triggered = true;

        // 1 — change Global Volume profile
        if (_globalVolume != null && _postTomeProfile != null)
            _globalVolume.profile = _postTomeProfile;

        // 2 — rise book into the air
        Vector3 startPos = transform.position;
        Vector3 riseTarget = startPos + Vector3.up * _riseHeight;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * _riseSpeed;
            transform.position = Vector3.Lerp(startPos, riseTarget, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        // Hold at top — rotate and bob
        bool peakTriggered = false;
        float hoverTimer = 0f;
        while (hoverTimer < _riseHoldDuration)
        {
            hoverTimer += Time.deltaTime;
            transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime, Space.World);
            float bobY = Mathf.Sin(hoverTimer * _bobFrequency * Mathf.PI * 2f) * _bobAmplitude;
            transform.position = riseTarget + new Vector3(0f, bobY, 0f);

            if (!peakTriggered && hoverTimer >= _riseHoldDuration - _emissionPeakFadeDuration)
            {
                peakTriggered = true;
                StartCoroutine(FadeEmission(_emissionIntensity, _emissionPeakIntensity, _emissionPeakFadeDuration));
            }

            yield return null;
        }

        // 3 — activate left hand with tome, hide the floating book
        yield return new WaitForSeconds(Mathf.Max(0f, _handActivateDelay - _riseHoldDuration));
        gameObject.SetActive(false);
        if (_leftHandWithTome != null) _leftHandWithTome.SetActive(true);

        // 4+5 — swap level design
        yield return new WaitForSeconds(Mathf.Max(0f, _levelSwapDelay - _handActivateDelay));
        if (_preTomeLevelDesign != null) _preTomeLevelDesign.SetActive(false);
        if (_postTomeLevelDesign != null) _postTomeLevelDesign.SetActive(true);

        // 6 — play voiceline
        if (!string.IsNullOrEmpty(_voicelineEvent))
        {
            GameObject emitter = _audioEmitter != null ? _audioEmitter : gameObject;
            AkSoundEngine.PostEvent(_voicelineEvent, emitter);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, _interactRange);
    }
#endif
}
