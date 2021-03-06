using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static Sprite CreateSpriteFromTexture(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f));
    }

    public static void DestroyAllChildren(Transform transform)
    {
        foreach (Transform t in transform)
        {
            GameObject.Destroy(t.gameObject);
        }
    }
}

public static class Colors
{
    public static Color WeaponColor => new Color(0.8f, 0.3f, 0.1f);
    public static Color ModifierColor => new Color(0.3f, 0.6f, 0.3f);
}
