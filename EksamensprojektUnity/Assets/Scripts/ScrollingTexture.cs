using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    [SerializeField] private Vector2 _scrollDirection = new Vector2(0f, -1f);
    [SerializeField] private float _speed = 0.5f;

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
    }
}
