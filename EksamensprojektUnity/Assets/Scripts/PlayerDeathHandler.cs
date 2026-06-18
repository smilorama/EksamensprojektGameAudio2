using System.Collections;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private GameObject _youDiedPanel;
    [SerializeField] private float _restartDelay = 3f;
    [SerializeField] private MonoBehaviour[] _componentsToDisableOnDeath;

    [Header("Death Fall")]
    [SerializeField] private GameObject _playerRoot;
    [SerializeField] private float _fallForce = 2f;

    [Header("Fade")]
    [SerializeField] private CanvasGroup _fadeCanvasGroup;
    [SerializeField] private float _fadeOutDuration = 1f;
    [SerializeField] private float _fadeInDuration = 0.5f;

    private void Start()
    {
        PlayerHealth health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        health.onDeath.AddListener(OnDeath);

        if (_youDiedPanel != null) _youDiedPanel.SetActive(false);

        if (_fadeCanvasGroup != null) StartCoroutine(FadeFromBlack());
    }

    private void OnDeath()
    {
        foreach (var c in _componentsToDisableOnDeath)
            if (c != null) c.enabled = false;

        MusicManager.Instance.SetStatePlayerDeath();

        if (_playerRoot != null)
        {
            CharacterController cc = _playerRoot.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            Rigidbody rb = _playerRoot.AddComponent<Rigidbody>();
            rb.AddForce((_playerRoot.transform.right + Vector3.up * 0.2f) * _fallForce, ForceMode.Impulse);
        }

        if (_youDiedPanel != null) _youDiedPanel.SetActive(true);
        StartCoroutine(RestartAfterDelay());
        if (_fadeCanvasGroup != null) StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeFromBlack()
    {
        _fadeCanvasGroup.alpha = 1f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _fadeInDuration;
            _fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        _fadeCanvasGroup.alpha = 0f;
    }

    private IEnumerator FadeToBlack()
    {
        _fadeCanvasGroup.alpha = 0f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _fadeOutDuration;
            _fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }
        _fadeCanvasGroup.alpha = 1f;
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(_restartDelay);
        Application.Quit();
    }
}
