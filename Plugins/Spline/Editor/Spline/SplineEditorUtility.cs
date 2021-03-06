using UnityEditor;
using UnityEngine;

class SplineEditorUtility
{
    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2f;
        EditorGUI.DrawRect(r, color);
    }
}
