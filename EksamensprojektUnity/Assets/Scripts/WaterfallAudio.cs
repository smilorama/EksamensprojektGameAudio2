using UnityEngine;

public class WaterfallAudio : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event _waterfallEvent;
    [SerializeField] private AK.Wwise.RTPC _pickupRTPC;
    [SerializeField] private float _pickupRTPCValue = 100f;

    private void Start()
    {
        if (_waterfallEvent.IsValid())
            _waterfallEvent.Post(gameObject);
    }

    public void OnTomePickup()
    {
        if (_pickupRTPC.IsValid())
            _pickupRTPC.SetValue(gameObject, _pickupRTPCValue);
    }
}
