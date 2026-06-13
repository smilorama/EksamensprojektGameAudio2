using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextGradient : MonoBehaviour
{
    [Header("Gradient")]
    [SerializeField] private Color _topColor = Color.white;
    [SerializeField] private Color _bottomColor = Color.black;

    [Header("Color Loop (bottom only)")]
    [SerializeField] private bool _loopColors = false;
    [SerializeField] private Color _loopColor1 = Color.red;
    [SerializeField] private Color _loopColor2 = Color.green;
    [SerializeField] private Color _loopColor3 = Color.blue;
    [SerializeField] private float _loopDuration = 2f;

    private TMP_Text _text;
    private float _loopTime;
    private Color[] _loopColorArray;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();

        _loopColorArray = new Color[] { _loopColor1, _loopColor2, _loopColor3 };
    }

    private void OnEnable()
    {
        _loopTime = 0f;
        ApplyGradient(_topColor, _bottomColor);
    }

    private void Update()
    {
        if (!_loopColors) return;

        _loopColorArray[0] = _loopColor1;
        _loopColorArray[1] = _loopColor2;
        _loopColorArray[2] = _loopColor3;

        float t = (_loopTime + Time.deltaTime / _loopDuration) % _loopColorArray.Length;
        _loopTime = t;

        int fromIndex = (int)t % _loopColorArray.Length;
        int toIndex = (fromIndex + 1) % _loopColorArray.Length;
        float blend = t - (int)t;

        Color bottom = Color.Lerp(_loopColorArray[fromIndex], _loopColorArray[toIndex], blend);

        ApplyGradient(_topColor, bottom);
    }

    private void ApplyGradient(Color top, Color bottom)
    {
        _text.colorGradient = new VertexGradient(top, top, bottom, bottom);
        _text.ForceMeshUpdate();
    }

    // Call this from code if you want to set a static gradient at runtime
    public void SetGradient(Color top, Color bottom)
    {
        _topColor = top;
        _bottomColor = bottom;
        if (!_loopColors)
            ApplyGradient(_topColor, _bottomColor);
    }
}
