using UnityEngine;

public class WaterfallAudio : MonoBehaviour
{
    public static bool TomePickedUp;

    [SerializeField] private AK.Wwise.Event _waterfallEvent;
    [SerializeField] private AK.Wwise.RTPC _pickupRTPC;
    [SerializeField] private float _pickupRTPCValue = 100f;

    [SerializeField] private Renderer[] _waterfallRenderers;

    private bool _rtpcSet;

    private void Start()
    {
        if (_waterfallEvent.IsValid())
            _waterfallEvent.Post(gameObject);
    }

    private void Update()
    {
        if (TomePickedUp && !_rtpcSet)
        {
            if (_pickupRTPC.IsValid())
                _pickupRTPC.SetValue(gameObject, _pickupRTPCValue);
            Color purple = new Color(0x4E / 255f, 0x00 / 255f, 0x79 / 255f);
            foreach (var r in _waterfallRenderers) if (r != null) r.material.color = purple;
            _rtpcSet = true;
        }
    }
}
