using UnityEditor;
using UnityEngine;

public class UIAnchorEditor : Editor
{
    // ---------- Corners to Anchors ----------
    [MenuItem("UI/Anchors to Corners &[")]
    static void AnchorsToCorners()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            RectTransform t = obj.GetComponent<RectTransform>();
            RectTransform p = obj.transform.parent.GetComponent<RectTransform>();
            if (t == null || p == null) continue;

            Vector2 newAnchMin = new Vector2(
                t.anchorMin.x + t.offsetMin.x / p.rect.width,
                t.anchorMin.y + t.offsetMin.y / p.rect.height
            );

            Vector2 newAnchMax = new Vector2(
                t.anchorMax.x + t.offsetMax.x / p.rect.width,
                t.anchorMax.y + t.offsetMax.y / p.rect.height
            );

            t.anchorMin = newAnchMin;
            t.anchorMax = newAnchMax;
            t.offsetMin = t.offsetMax = Vector2.zero;
        }
    }

    // ---------- Canvas Group Alpha ----------

    [MenuItem("UI/Set CanvasGroup Alpha &A")]

    static void SetCanvasGroupAlpha()
    {

        foreach (GameObject obj in Selection.gameObjects)
        {
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();
            if (cg == null) continue;
            if (cg.alpha != 1f)
                cg.alpha = 1f;
            else
                cg.alpha = 0f;
            EditorUtility.SetDirty(cg);
        }
    }
}
