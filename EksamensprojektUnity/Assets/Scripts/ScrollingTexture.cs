using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    [SerializeField] private Vector2 _scrollDirection = new Vector2(0f, -1f);
    [SerializeField] private float _speed = 0.5f;

    [Header("Edge Fade")]
    [SerializeField] private bool _edgeFade = false;
    [SerializeField] private float _edgeFadeWidth = 0.2f;

    private Material _mat;
    private string _texProperty;
    private Vector2 _offset;

    private static readonly string[] _candidates = { "_BaseMap", "_MainTex", "_BaseColorMap" };

    private void Start()
    {
        _mat = GetComponent<Renderer>().material;

        foreach (string prop in _candidates)
        {
            if (_mat.HasProperty(prop))
            {
                _texProperty = prop;
                break;
            }
        }

        if (_texProperty == null)
            Debug.LogWarning($"ScrollingTexture: ingen kendt texture property fundet på {_mat.shader.name}", this);
    }

    private void Update()
    {
        if (_texProperty == null) return;
        _offset += _scrollDirection.normalized * _speed * Time.deltaTime;
        _mat.SetTextureOffset(_texProperty, _offset);

        if (_edgeFade && _mat.HasProperty("_BaseColor"))
        {
            // Sample UV center — kant-fade via mesh bounds er ikke tilgængeligt direkte,
            // så vi bruger en simpel vertikal fade baseret på scrolloffset mod kanten
            float uv = Mathf.Repeat(_offset.y, 1f);
            float fade = Mathf.Min(
                Mathf.InverseLerp(0f, _edgeFadeWidth, uv),
                Mathf.InverseLerp(1f, 1f - _edgeFadeWidth, uv)
            );
            Color c = _mat.GetColor("_BaseColor");
            c.a = fade;
            _mat.SetColor("_BaseColor", c);
        }
    }
}
