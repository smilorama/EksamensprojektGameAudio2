using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TomeLeaveRoom : MonoBehaviour
{
    public enum TriggerType { DoorBlocker, OutsideTrigger }

    [Header("Type")]
    [SerializeField] private TriggerType _type;

    [Header("Door Blocker (activate on exit)")]
    [SerializeField] private GameObject _blockerObject;
    [SerializeField] private Material _outsideSkybox;

    [Header("Outside Trigger (remove tome + dialogue)")]
    [SerializeField] private GameObject _tomeInPlayerHand;
    [SerializeField] private string _dialogueSoundEvent = "";
    [SerializeField] private GameObject _audioEmitter;

    private bool _triggered;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        if (_blockerObject != null) _blockerObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered || !other.CompareTag("Player")) return;
        if (!MusicGameProgressManager.TomePickedUp) return;

        _triggered = true;

        if (_type == TriggerType.DoorBlocker)
        {
            if (_blockerObject != null) _blockerObject.SetActive(true);
            if (_outsideSkybox != null) RenderSettings.skybox = _outsideSkybox;
        }
        else if (_type == TriggerType.OutsideTrigger)
        {
            if (_tomeInPlayerHand != null) _tomeInPlayerHand.SetActive(false);
            MusicGameProgressManager.TomePickedUp = false;
            DialogueUI.Instance.SetFlag("Goddess Freed");

            if (!string.IsNullOrEmpty(_dialogueSoundEvent))
            {
                GameObject emitter = _audioEmitter != null ? _audioEmitter : gameObject;
                AkUnitySoundEngine.PostEvent(_dialogueSoundEvent, emitter);
            }
        }
    }
}
