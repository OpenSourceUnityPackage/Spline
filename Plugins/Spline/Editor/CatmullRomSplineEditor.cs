using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(CatmullRomSpline))]
public class CatmullRomSplineEditor : Editor
{
    // Custom in-scene UI for when Spline script
    // component is selected.
    private float pointSize = 0.1f;
    private int splineDivision = 20;
    private bool isExtremityAdd = false;
    private CatmullRomSpline self = null;
    
    
    private void OnEnable()
    {
        self = target as CatmullRomSpline;
    }

    protected virtual void OnSceneGUI()
    {
        if (!self.enabled)
            return;
        
        if (self.points.Count > 3)
            Handles.DrawAAPolyLine(self.MakeSplinePoints(splineDivision));
        
        for (int i = 0; i < self.points.Count; i++)
        {
            Handles.color = Color.green;
            Vector3 newPos = Handles.FreeMoveHandle( self.points[i].point, Quaternion.identity,
                HandleUtility.GetHandleSize( self.points[i].point) * pointSize, Vector3.one, Handles.SphereHandleCap);
            
            self.points[i] = new CatmullRomSpline.Point{point = newPos};
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SplineEditorUtility.DrawUILine(Color.gray);
        GUILayout.Label("Editor settings :");
        pointSize = EditorGUILayout.FloatField("Point size", pointSize);
        EditorGUI.BeginChangeCheck();
        splineDivision = EditorGUILayout.IntField("Curve precision", splineDivision);
        if (EditorGUI.EndChangeCheck())
        {
            splineDivision = Mathf.Clamp(splineDivision, 3, 1000);
            EditorUtility.SetDirty(target);
        }

        if (self.points.Count > 3)
        {
            EditorGUI.BeginChangeCheck();
            isExtremityAdd = EditorGUILayout.Toggle("Include extremity", isExtremityAdd);
            if (EditorGUI.EndChangeCheck())
            {
                if (isExtremityAdd)
                {
                    self.points.Insert(0, self.points.First());
                    self.points.Add(self.points.Last());
                }
                else
                {
                    self.points.RemoveAt(0);
                    self.points.RemoveAt(self.points.Count - 1);
                }
                EditorUtility.SetDirty(target);
            }
        }
    }
}