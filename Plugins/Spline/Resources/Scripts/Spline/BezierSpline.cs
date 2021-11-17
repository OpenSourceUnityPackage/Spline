using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class BezierSpline : Spline
{
    private static Matrix4x4 constantMatrix =
        new Matrix4x4(
            new Vector4(-1f, 3f, -3f, 1f),
            new Vector4(3f, -6f, 3f, 0f),
            new Vector4(-3f, 3f, 0f, 0f),
            new Vector4(1f, 0f, 0f, 0f)
        );

    [System.Serializable]
    public struct Point
    {
        public Vector3 point;
    }
    
#if UNITY_EDITOR
    [HideInInspector] public bool isExtremityAdd = false;
    [HideInInspector] public bool isContinues = true;
#endif
    
    public List<Point> points = new List<Point>();
    
    public override Vector3 GetLocalInterpolation(int pointIndex, float t)
    {
        t = Mathf.Clamp01(t);

        Vector4 tVec = new Vector4(t*t*t, t*t, t, 1f);
        Matrix4x4 pointsMatrix = new Matrix4x4(
            points[pointIndex].point,
            points[pointIndex + 1].point,
            points[pointIndex + 2].point,
            points[pointIndex + 3].point);

        return pointsMatrix * constantMatrix * tVec;
    }

    public override Vector3[] MakeSplinePoints(int divisionBySpline)
    {
        if (points.Count < 3)
            return null;
        
        int totalPoint = (points.Count - points.Count % 4) / 4;
        Vector3[] pointsRst = new Vector3[divisionBySpline * totalPoint + 1];
        float step = 1f / divisionBySpline;

        for (int i = 0; i < totalPoint; i++)
        {
            float t = 0f;
            for (int j = 0; j < divisionBySpline; j++)
            {
                pointsRst[i * divisionBySpline + j] = GetLocalInterpolation(i * 4, t);
                t += step;
            }
        }
        
        // Include the last point
        pointsRst[pointsRst.Length - 1] = GetLocalInterpolation((totalPoint - 1) * 4, 1f);

        return pointsRst;
    }
    
    public override Vector3[] MakeLocalSplinePoints(int pointIndex, int divisionBySpline)
    {
        if (!IsIndexValid(pointIndex))
            return null;
        
        Vector3[] pointsRst = new Vector3[divisionBySpline + 1];
        float step = 1f / divisionBySpline;

        float t = 0f;
        for (int j = 0; j < pointsRst.Length; j++)
        {
            pointsRst[j] = GetLocalInterpolation(pointIndex, t);
            t += step;
        }

        return pointsRst;
    }
    
    public override void Save(string dst)
    {
        using (StreamWriter writer = new StreamWriter(dst))
        {
            writer.WriteLine(JsonHelper.ToJson(points.ToArray(), true));
            writer.Close();
        }
    }
    
    public override void Load(string src)
    {
        using (StreamReader reader = new StreamReader(src))
        {
            points = new List<Point>(JsonHelper.FromJson<Point>(reader.ReadToEnd()));
            reader.Close();
        }
    }
    
    public override int GetMaxIndex()
    {
        return points.Count;
    }
    
    public override int GetMinIndex()
    {
        return 0;
    }
    
    public override int GetIndexStep()
    {
        return 4;
    }
}