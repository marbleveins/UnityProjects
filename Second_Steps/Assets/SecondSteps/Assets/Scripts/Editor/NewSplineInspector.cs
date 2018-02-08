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

    private bool splineOn = false;

    private NewSpline spline;

    private int selectedNodeIndex;
    private Node selectedNode;

    private void OnSceneGUI()
    {
        spline = target as NewSpline;

        if (splineOn)
        {
            handleTransform = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            DrawHandles();
            DrawCurves();
        }
        else
        {
            StartNewSpline();
        }

    }


    private void StartNewSpline()
    {
        spline.AddNewDefaultCurve();





        splineOn = true;
    }


    private void DrawHandles()
    {
        foreach(Node node in spline.nodes)
        {
            Handles.Button(node.location, handleRotation, 2f * handleSize, 2f * pickSize, Handles.DotHandleCap);
        }
    }

    private void DrawCurves()
    {
        for (int i = 0; i < spline.curves.Count; i++)
        {
            Handles.DrawBezier(spline.transform.TransformPoint(spline.curves[i].n1.location),
                spline.transform.TransformPoint(spline.curves[i].n2.location),
                spline.transform.TransformPoint(spline.curves[i].n1.direction),
                spline.transform.TransformPoint(spline.curves[i].GetInverseDirection()),
                Color.white,
                null,
                2f);
            Handles.DrawLine(spline.curves[i].n1.location, spline.curves[i].c1.location);
            Handles.DrawLine(spline.curves[i].n2.location, spline.curves[i].c2.location);
        }
    }

}
