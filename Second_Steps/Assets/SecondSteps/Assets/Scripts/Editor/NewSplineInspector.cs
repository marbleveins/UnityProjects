using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(NewSpline))]
public class NewSplineInspector : Editor {

    private Quaternion handleRotation;
    private Transform handleTransform;
    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;
    private const float directionineScale = 0.8f;
    

    private NewSpline spline;
    



    private void OnSceneGUI()
    {
        spline = target as NewSpline;
        
        if (spline.points.Count > 0)
        {
            handleTransform = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            DrawHandles();
            DrawCurves();
            DrawDirections();
        }
        else
        {
            StartNewSpline();
        }
        
    }


    private void StartNewSpline()
    {
        spline.points.Clear();
        spline.curves.Clear();
        spline.AddDefaultCurve();
                
    }


    private void DrawHandles()
    {
        foreach (Node node in spline.points)
        {
            Handles.Button(node.position, handleRotation, 2f * handleSize, 2f * pickSize, Handles.DotHandleCap);
        }
    }

    private void DrawCurves()
    {
        foreach(Curve curve in spline.curves)
        {
            Handles.DrawBezier(spline.transform.TransformPoint(curve.nodoInicio.position),
                spline.transform.TransformPoint(curve.nodoFin.position),
                spline.transform.TransformPoint(curve.c1.position),
                spline.transform.TransformPoint(curve.GetInverseDirection()),
                Color.white,
                null,
                2f);
            Handles.DrawLine(curve.nodoInicio.position, curve.c1.position);
            Handles.DrawLine(curve.nodoFin.position, curve.c2.position);
        }
    }

    private void DrawDirections()
    {
        if (spline.points.Count > 0)
        {
            Handles.color = Color.green;

            Vector3 point = spline.GetPositionAtTime(0f);
            Handles.DrawLine(point, point + spline.GetOrientationAtTime(0f) * directionineScale);
            int steps = spline.breaks * spline.curves.Count;
            for (int i = 1; i <= steps; i++)
            {
                point = spline.GetPositionAtTime(i / (float)steps);
                Handles.DrawLine(point, point + spline.GetOrientationAtTime(i / (float)steps) * directionineScale);
            }

        }
    }

}
