using UnityEngine;
using UnityEngine.UI;

// Horizontal gradient UI element — left color to right color.
// Drop-in replacement for Image when you want a gradient fill.

public class GradientImage : MaskableGraphic
{
    private Color _left  = Color.red;
    private Color _right = new Color(0.4f, 0f, 0f);

    public void SetGradient(Color left, Color right)
    {
        _left  = left;
        _right = right;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect r = rectTransform.rect;

        // bottom-left, bottom-right, top-right, top-left
        vh.AddVert(new Vector3(r.xMin, r.yMin), _right, Vector2.zero);
        vh.AddVert(new Vector3(r.xMax, r.yMin), _right, Vector2.zero);
        vh.AddVert(new Vector3(r.xMax, r.yMax), _left,  Vector2.zero);
        vh.AddVert(new Vector3(r.xMin, r.yMax), _left,  Vector2.zero);

        vh.AddTriangle(0, 3, 2);
        vh.AddTriangle(0, 2, 1);
    }
}
