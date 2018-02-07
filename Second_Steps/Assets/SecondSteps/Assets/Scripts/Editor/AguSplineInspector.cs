using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AguSpline))]
public class AguSplineInspector : Editor
{

    private AguSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const int stepsPerCurve = 10;
    private const float directionScale = 0.5f;

    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;

    private int selectedIndex = -1;

    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    private void OnSceneGUI()
    {
        spline = target as AguSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;
        Debug.Log("OnSceneGUI");


        Vector3 p0;
        if (spline.NodeExists(0))
        {
            p0 = ShowPoint(0);
        
            for (int i = 1; i < spline.ControlPointsCount; i += 3)
            {
                Vector3 p1 = ShowPoint(i);
                Vector3 p2 = ShowPoint(i + 1);
                Vector3 p3 = ShowPoint(i + 2);

                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
                p0 = p3;
            }
        }
        ShowDirections();
    }

    public override void OnInspectorGUI()
    {
        spline = target as AguSpline;
        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointsCount)
        {
            DrawSelectedPointInspector();
        }
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Debug.Log("primerControlP");
        Vector3 loc = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex).position);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPointLocation(selectedIndex, loc);
        }

        EditorGUI.BeginChangeCheck();
        ControlPointMode mode = (ControlPointMode) EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Change Point Mode");
            spline.SetControlPointMode(selectedIndex, mode);
            EditorUtility.SetDirty(spline);
        }
    }

    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = spline.GetLocationAlongSpline(0f);
        Handles.DrawLine(point, point + spline.GetOrientationAtTime(0f) * directionScale);
        int steps = stepsPerCurve * spline.curves.Count;
        for (int i = 1; i <= steps; i++)
        {
            point = spline.GetLocationAlongSpline(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetOrientationAtTime(i / (float)steps) * directionScale);
        }
    }

    private Vector3 ShowPoint(int index)
    {
        Debug.Log("ShowPoint: " + index);

        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index).position);
        float size = HandleUtility.GetHandleSize(point);
        if (index == 0)
        {
            size *= 2f;
        }
        Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
        if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
        {
            selectedIndex = index;
            Repaint();
        }
        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPointLocation(index, handleTransform.InverseTransformPoint(point));
            }
        }
        return point;
    }

}