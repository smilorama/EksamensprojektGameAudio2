using System.Collections;
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
    [SerializeField] private string _roomToneRTPC = "";
    [SerializeField] private float _roomToneRTPCValue = 0f;

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
            if (!string.IsNullOrEmpty(_roomToneRTPC))
                AkUnitySoundEngine.SetRTPCValue(_roomToneRTPC, _roomToneRTPCValue);
        }
        else if (_type == TriggerType.OutsideTrigger)
        {
            if (_tomeInPlayerHand != null) _tomeInPlayerHand.SetActive(false);
            MusicGameProgressManager.TomePickedUp = false;
            DialogueUI.Instance.SetFlag("Goddess Freed");
            MusicManager.Instance.SetStateTomeOutside(DialogueUI.Instance != null && DialogueUI.Instance.HasFlag("Tome Burned"));
            StartCoroutine(FadeMusicVolume());

            if (!string.IsNullOrEmpty(_dialogueSoundEvent) && (DialogueUI.Instance == null || !DialogueUI.Instance.HasFlag("Tome Burned")))
            {
                GameObject emitter = _audioEmitter != null ? _audioEmitter : gameObject;
                AkUnitySoundEngine.PostEvent(_dialogueSoundEvent, emitter);
            }
        }
    }

    private IEnumerator FadeMusicVolume()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 4f;
            AkUnitySoundEngine.SetRTPCValue("MusicVolume", Mathf.Lerp(0f, 100f, t));
            yield return null;
        }
        AkUnitySoundEngine.SetRTPCValue("MusicVolume", 100f);
    }
}
