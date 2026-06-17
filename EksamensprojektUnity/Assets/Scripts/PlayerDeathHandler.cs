using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private GameObject _youDiedPanel;
    [SerializeField] private float _restartDelay = 3f;
    [SerializeField] private MonoBehaviour[] _componentsToDisableOnDeath;

    private void Start()
    {
        PlayerHealth health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        health.onDeath.AddListener(OnDeath);

        if (_youDiedPanel != null) _youDiedPanel.SetActive(false);
    }

    private void OnDeath()
    {
        foreach (var c in _componentsToDisableOnDeath)
            if (c != null) c.enabled = false;

        if (_youDiedPanel != null) _youDiedPanel.SetActive(true);
        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(_restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
