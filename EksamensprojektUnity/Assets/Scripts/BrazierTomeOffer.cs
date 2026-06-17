using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class BrazierTomeOffer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _tomeInPlayerHand;
    [SerializeField] private GameObject _brazierTomeRenderer;
    [SerializeField] private float _interactRange = 2f;
    [SerializeField] private GameObject _promptPanel;

    [Header("Emission Fade")]
    [SerializeField] private Color _emissionColor = new Color(1f, 0.3f, 0f);
    [SerializeField] private float _emissionStartIntensity = 1f;
    [SerializeField] private float _emissionMaxIntensity = 8f;
    [SerializeField] private float _emissionFadeDuration = 1.5f;

    [Header("Post Processing")]
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private VolumeProfile _preTomeProfile;

    [Header("After Burn")]
    [SerializeField] private Material _postBurnSkybox;
    [SerializeField] private float _postBurnFogDensity = 0.01f;

    [Header("Audio")]
    [SerializeField] private string _burnEvent = "";
    [SerializeField] private string _restoreEvent = "";
    [SerializeField] private GameObject _audioEmitter;


    private Transform _player;
    private bool _used;

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _player = p.transform;

        if (_brazierTomeRenderer != null) _brazierTomeRenderer.SetActive(false);
        if (_promptPanel != null) _promptPanel.SetActive(false);
    }

    private void Update()
    {
        if (_used || _player == null || !MusicGameProgressManager.TomePickedUp) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        bool inRange = dist <= _interactRange;

        if (_promptPanel != null && _promptPanel.activeSelf != inRange)
            _promptPanel.SetActive(inRange);

        if (inRange && Keyboard.current.fKey.wasPressedThisFrame)
            StartCoroutine(BurnSequence());
    }

    private IEnumerator BurnSequence()
    {
        _used = true;
        if (_promptPanel != null) _promptPanel.SetActive(false);

        // Remove tome from player hand, show in brazier
        if (_tomeInPlayerHand != null) _tomeInPlayerHand.SetActive(false);
        if (_brazierTomeRenderer != null) _brazierTomeRenderer.SetActive(true);

        if (!string.IsNullOrEmpty(_burnEvent))
        {
            GameObject emitter = _audioEmitter != null ? _audioEmitter : gameObject;
            AkUnitySoundEngine.PostEvent(_burnEvent, emitter);
        }

        // Fade emission from start to max then deactivate
        Renderer rend = _brazierTomeRenderer.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = rend.material;
            mat.EnableKeyword("_EMISSION");
            float h, s, v;
            Color.RGBToHSV(_emissionColor, out h, out s, out v);
            Vector3 basePos = _brazierTomeRenderer.transform.localPosition;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / _emissionFadeDuration;

                // Emission fade — exponential curve so it explodes toward the end
                float intensity = Mathf.Lerp(_emissionStartIntensity, _emissionMaxIntensity, t * t * t);
                mat.SetColor("_EmissionColor", Color.HSVToRGB(h, s, 1f, true) * intensity);

                // Face player on Y axis
                Vector3 dir = _player.position - _brazierTomeRenderer.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                    _brazierTomeRenderer.transform.rotation = Quaternion.LookRotation(dir);

                // Vibrate
                Vector3 shake = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 0.01f;
                _brazierTomeRenderer.transform.localPosition = basePos + shake;

                yield return null;
            }

            _brazierTomeRenderer.transform.localPosition = basePos;
        }

        if (_brazierTomeRenderer != null) _brazierTomeRenderer.SetActive(false);

        if (!string.IsNullOrEmpty(_restoreEvent))
        {
            GameObject emitter = _audioEmitter != null ? _audioEmitter : gameObject;
            AkUnitySoundEngine.PostEvent(_restoreEvent, emitter);
        }

        MusicGameProgressManager.TomePickedUp = false;
        DialogueUI.Instance.SetFlag("Tome Burned");
        MusicManager.Instance.SetStateTomeBraending();

        if (_globalVolume != null && _preTomeProfile != null)
            _globalVolume.profile = _preTomeProfile;

        if (_postBurnSkybox != null)
            RenderSettings.skybox = _postBurnSkybox;
        RenderSettings.fogDensity = _postBurnFogDensity;
    }
}
