using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Enemy))]
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Enemy Info")]
    [SerializeField] private string _enemyName       = "Enemy";
    [SerializeField] private TMP_FontAsset _nameFont;

    [Header("Bar Appearance")]
    [SerializeField] private Color _barFillColor     = new Color(0.55f, 0f, 0f);
    [SerializeField] private Color _bgColor          = new Color(0.08f, 0f, 0f, 0.75f);
    [SerializeField] private Vector2 _barSize        = new Vector2(260f, 14f);

    [Header("Name Appearance")]
    [SerializeField] private Color _nameTopColor     = Color.white;
    [SerializeField] private Color _nameBottomColor  = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private float _nameFontSize     = 16f;

    [Header("Screen Position (bottom-right)")]
    [Tooltip("Pixel offset from bottom-right corner")]
    [SerializeField] private Vector2 _anchorOffset   = new Vector2(-40f, 60f);

    [Header("Visibility")]
    [SerializeField] private float _fadeSpeed            = 8f;
    [SerializeField] private float _showDurationAfterHit = 4f;

    private Enemy           _enemy;
    private CanvasGroup     _canvasGroup;
    private RectTransform   _fillRect;
    private TextMeshProUGUI _nameText;

    private float _lastHealth;
    private float _showUntil;
    private bool  _dead;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        BuildUI();
        _lastHealth = _enemy.CurrentHealth;
    }

    private void LateUpdate()
    {
        if (_dead) { Fade(false); return; }

        float hp = _enemy.CurrentHealth;
        if (hp <= 0f) { _dead = true; Fade(false); return; }

        if (hp < _lastHealth)
            _showUntil = Time.time + _showDurationAfterHit;
        _lastHealth = hp;

        Fade(_enemy.IsAggro || Time.time < _showUntil);

        float pct = Mathf.Clamp01(hp / Mathf.Max(0.001f, _enemy.MaxHealth));
        _fillRect.localScale = new Vector3(pct, 1f, 1f);
    }

    private void Fade(bool show)
    {
        if (_canvasGroup == null) return;
        _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, show ? 1f : 0f, _fadeSpeed * Time.deltaTime);
    }

    private void BuildUI()
    {
        var rootGO = new GameObject("EnemyHUD");
        rootGO.transform.SetParent(transform, false);

        var canvas = rootGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        var scaler = rootGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;

        rootGO.AddComponent<GraphicRaycaster>();

        var canvasRect      = rootGO.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = canvasRect.offsetMax = Vector2.zero;

        // container — bottom-right
        var containerGO   = new GameObject("Container");
        containerGO.transform.SetParent(rootGO.transform, false);
        var containerRect = containerGO.AddComponent<RectTransform>();
        containerRect.anchorMin        = new Vector2(1f, 0f);
        containerRect.anchorMax        = new Vector2(1f, 0f);
        containerRect.pivot            = new Vector2(1f, 0f);
        containerRect.anchoredPosition = _anchorOffset;
        containerRect.sizeDelta        = new Vector2(_barSize.x, _barSize.y + 28f);

        _canvasGroup                = containerGO.AddComponent<CanvasGroup>();
        _canvasGroup.alpha          = 0f;
        _canvasGroup.blocksRaycasts = false;

        // nameplate with gradient
        var nameGO = new GameObject("Name");
        nameGO.transform.SetParent(containerGO.transform, false);
        _nameText           = nameGO.AddComponent<TextMeshProUGUI>();
        _nameText.text      = _enemyName;
        _nameText.fontSize  = _nameFontSize;
        _nameText.alignment = TextAlignmentOptions.Right;
        if (_nameFont != null) _nameText.font = _nameFont;
        _nameText.enableVertexGradient = true;
        _nameText.colorGradient = new VertexGradient(
            _nameTopColor,   _nameTopColor,
            _nameBottomColor, _nameBottomColor
        );
        var nameRect = nameGO.GetComponent<RectTransform>();
        nameRect.anchorMin        = new Vector2(0f, 1f);
        nameRect.anchorMax        = new Vector2(1f, 1f);
        nameRect.pivot            = new Vector2(1f, 0f);
        nameRect.anchoredPosition = new Vector2(0f, 4f);
        nameRect.sizeDelta        = new Vector2(0f, 22f);

        // bar background
        var barGO   = new GameObject("Bar");
        barGO.transform.SetParent(containerGO.transform, false);
        var bgImage = barGO.AddComponent<Image>();
        bgImage.color         = _bgColor;
        bgImage.raycastTarget = false;
        bgImage.sprite        = WhiteSprite();
        var barRect = barGO.GetComponent<RectTransform>();
        barRect.anchorMin        = new Vector2(0f, 0f);
        barRect.anchorMax        = new Vector2(1f, 0f);
        barRect.pivot            = new Vector2(0.5f, 0f);
        barRect.anchoredPosition = Vector2.zero;
        barRect.sizeDelta        = new Vector2(0f, _barSize.y);

        // fill
        var fillGO    = new GameObject("Fill");
        fillGO.transform.SetParent(barGO.transform, false);
        var fillImage = fillGO.AddComponent<Image>();
        fillImage.color         = _barFillColor;
        fillImage.raycastTarget = false;
        fillImage.sprite        = WhiteSprite();

        _fillRect           = fillGO.GetComponent<RectTransform>();
        _fillRect.anchorMin = new Vector2(0f, 0f);
        _fillRect.anchorMax = new Vector2(0f, 1f);
        _fillRect.pivot     = new Vector2(0f, 0.5f);
        _fillRect.offsetMin = Vector2.zero;
        _fillRect.offsetMax = new Vector2(_barSize.x, 0f);
    }

    private static Sprite WhiteSprite()
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
    }
}
