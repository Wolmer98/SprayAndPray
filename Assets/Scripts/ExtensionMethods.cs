using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 AddVec2(this Vector3 a, Vector2 b)
    {
        a.x += b.x;
        a.y += b.y;
        return a;
    }
}
