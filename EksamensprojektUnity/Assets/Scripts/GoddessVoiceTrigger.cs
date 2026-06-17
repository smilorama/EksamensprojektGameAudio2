using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GoddessVoiceTrigger : MonoBehaviour
{
    [SerializeField] private string _voiceEvent = "";
    [SerializeField] private GameObject _audioEmitter;
    [SerializeField] private bool _silenceIfTomeBurned = false;
    [SerializeField] private bool _requireGoddessFreed = false;

    private bool _triggered;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered || !other.CompareTag("Player")) return;

        if (DialogueUI.Instance == null) return;
        if (_silenceIfTomeBurned && DialogueUI.Instance.HasFlag("Tome Burned")) return;
        if (_requireGoddessFreed && !DialogueUI.Instance.HasFlag("Goddess Freed")) return;

        _triggered = true;

        if (!string.IsNullOrEmpty(_voiceEvent))
        {
            GameObject emitter = _audioEmitter != null ? _audioEmitter : gameObject;
            AkUnitySoundEngine.PostEvent(_voiceEvent, emitter);
        }
    }
}
