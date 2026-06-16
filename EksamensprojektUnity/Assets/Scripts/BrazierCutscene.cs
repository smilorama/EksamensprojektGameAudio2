using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider))]
public class BrazierCutscene : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField] private Transform _npc;
    [SerializeField] private Animator _npcAnimator;
    [SerializeField] private Transform _brazierDestination;
    [SerializeField] private float _arrivalDistance = 0.4f;
    [SerializeField] private float _walkSpeed = 1.5f;

    [Header("Tome")]
    [SerializeField] private GameObject _tomeInHand;

    [Header("Timing")]
    [SerializeField] private float _arrivalPause = 1f;
    [SerializeField] private float _tomeDelay = 2f;
    [SerializeField] private float _igniteDelay = 0.5f;

    [Header("Fire on NPC")]
    [SerializeField] private GameObject _npcFireEffect;
    [SerializeField] private float _fireEmissionIntensity = 2f;
    [SerializeField] private Color _fireEmissionColor = new Color(1f, 0.5f, 0.1f);

    [Header("Mushroom")]
    [SerializeField] private Renderer _mushroomRenderer;
    [SerializeField] private float _mushroomFadeDuration = 2f;

    [Header("Post Processing")]
    [SerializeField] private Volume _cutsceneVolume;
    [SerializeField] private float _volumeFadeInDuration = 1f;
    [SerializeField] private float _volumeFadeOutDuration = 2f;
    [SerializeField] private float _volumeHoldAfterBurning = 5f;

    [Header("Audio")]
    [SerializeField] private string _burnEvent = "";
    [SerializeField] private GameObject _audioEmitter;

    private bool _triggered;
    private Material _mushroomMat;
    private Color _mushroomEmissionStart;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        if (_cutsceneVolume != null) _cutsceneVolume.weight = 0f;

        if (_mushroomRenderer != null)
        {
            _mushroomEmissionStart = _mushroomRenderer.sharedMaterial.GetColor("_EmissionColor");
            _mushroomMat = _mushroomRenderer.material;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered || !other.CompareTag("Player")) return;
        _triggered = true;
        StartCoroutine(RunCutscene());
    }

    private IEnumerator RunCutscene()
    {
        // Fade in cutscene volume
        if (_cutsceneVolume != null)
            StartCoroutine(FadeVolume(0f, 1f, _volumeFadeInDuration));

        // Fade out mushroom emission
        if (_mushroomRenderer != null)
            StartCoroutine(FadeMushroomEmission(_mushroomFadeDuration));

        // Face destination before walking
        Vector3 toTarget = _brazierDestination.position - _npc.position;
        toTarget.y = 0f;
        if (toTarget.sqrMagnitude > 0.001f)
            _npc.rotation = Quaternion.LookRotation(toTarget);

        _npcAnimator.SetFloat("Vertical", 1f);
        _npcAnimator.SetBool("isMoving", true);

        while (Vector3.Distance(_npc.position, _brazierDestination.position) > _arrivalDistance)
        {
            _npc.position = Vector3.MoveTowards(_npc.position, _brazierDestination.position, _walkSpeed * Time.deltaTime);
            yield return null;
        }

        _npc.position = new Vector3(_brazierDestination.position.x, _npc.position.y, _brazierDestination.position.z);
        _npcAnimator.SetFloat("Vertical",   0f);
        _npcAnimator.SetFloat("Horizontal", 0f);
        _npcAnimator.SetBool("isMoving",    false);

        // Face brazier
        Vector3 dir = _brazierDestination.position - _npc.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            _npc.rotation = Quaternion.LookRotation(dir);

        yield return new WaitForSeconds(_arrivalPause);

        _npcAnimator.SetLayerWeight(1, 1f);
        _npcAnimator.Play("Place Object", 1, 0f);
        yield return new WaitForSeconds(_tomeDelay);

        if (_tomeInHand != null) _tomeInHand.SetActive(false);

        yield return new WaitForSeconds(_igniteDelay);

        if (_npcFireEffect != null)
        {
            _npcFireEffect.SetActive(true);

            foreach (var r in _npcFireEffect.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                if (r.sharedMaterial == null) continue;
                Material mat = new Material(r.sharedMaterial);
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", _fireEmissionColor * _fireEmissionIntensity);
                r.material = mat;
            }
        }

        _npcAnimator.Play("Burning", 1, 0f);

        if (!string.IsNullOrEmpty(_burnEvent))
        {
            GameObject emitter = _audioEmitter != null ? _audioEmitter : _npc.gameObject;
            AkUnitySoundEngine.PostEvent(_burnEvent, emitter);
        }

        // Hold then fade volume back out
        if (_cutsceneVolume != null)
        {
            yield return new WaitForSeconds(_volumeHoldAfterBurning);
            yield return StartCoroutine(FadeVolume(1f, 0f, _volumeFadeOutDuration));
        }
        else
            yield return new WaitForSeconds(_volumeHoldAfterBurning);

        if (_npcFireEffect != null) _npcFireEffect.SetActive(false);
        if (_npc != null) _npc.gameObject.SetActive(false);
    }

    private IEnumerator FadeMushroomEmission(float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            _mushroomMat.SetColor("_EmissionColor", Color.Lerp(_mushroomEmissionStart, Color.black, t));
            yield return null;
        }
        _mushroomMat.SetColor("_EmissionColor", Color.black);
    }

    private IEnumerator FadeVolume(float from, float to, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            _cutsceneVolume.weight = Mathf.Lerp(from, to, t);
            yield return null;
        }
        _cutsceneVolume.weight = to;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_brazierDestination == null) return;
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.8f);
        Gizmos.DrawSphere(_brazierDestination.position, 0.2f);
        Gizmos.DrawLine(transform.position, _brazierDestination.position);
    }
#endif
}
