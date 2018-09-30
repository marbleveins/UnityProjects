using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class SplineClass : ScriptableObject
{
    public List<Node> points = new List<Node>();
    public List<Curve> curves = new List<Curve>();

    //private List<Node> GetPoints()
    //{
    //    return new List<Node>().AddRange()
    //}
    public int breaks = 10;
    public bool breaksFixedOnlyTrue = true;
    public bool relativePosition = true;
    

    /// <summary>
    /// Public Functions
    /// </summary>
    #region Public Functions

    public void AddCurve(Curve cu)
    {
        points.Add(cu.nodoInicio);
        points.Add(cu.c1);
        points.Add(cu.c2);
        points.Add(cu.nodoFin);
        curves.Add(cu);
    }
    public Vector3 GetPositionAtTime(float t)
    {
        int curveIndex = GetCurveIndexForTime(t);
        return curves[curveIndex].GetLocation(t - curveIndex);
    }
    public Vector3 GetOrientationAtTime(float t)//GetTangentAlongSpline
    {
        int index = GetCurveIndexForTime(t);
        return curves[index].GetTangent(t - index);
    }

    public List<OrientedPoint> GetPath()
    {
        var path = new List<OrientedPoint>();
        for (float t = 0; t < curves.Count; t += 1 / (float)breaks)
        {
            Debug.Log(string.Format("t: {0} ", t));
            var point = GetPositionAtTime(t);
            var rotation = Curve.GetRotationFromTangent(GetOrientationAtTime(t));
            path.Add(new OrientedPoint(point, rotation));
        }
        return path;
    }
    public List<OrientedPoint> GetPath2()
    {
        var path = new List<OrientedPoint>();
        float t = 0;
        for (float i = 0; i <= curves.Count * breaks; i += 1)
        {
            t = i / breaks;
            Debug.Log(string.Format("t: {0} ", t));
            var point = GetPositionAtTime(t);
            var rotation = Curve.GetRotationFromTangent(GetOrientationAtTime(t));
            path.Add(new OrientedPoint(point, rotation));
        }
        return path;
    }

    #endregion


    /// <summary>
    /// Private Methods
    /// </summary>
    #region Private Methods


    private int GetCurveIndexForTime(float t)
    {
        if (t < 0 || t > curves.Count)
        {
            throw new ArgumentException(string.Format("Time must be between 0 and last node index ({0}). Given time was {1}.", curves.Count + 1, t));
        }
        int index = Mathf.FloorToInt(t);
        if (index == curves.Count)
            index--;// ej;si hay una sola curva, t = 1 tiene que devolver index 0
        return index;
    }

    private int NodeCountOnly
    {
        get { return points.Count - curves.Count * 2; }
    }

    #endregion

}