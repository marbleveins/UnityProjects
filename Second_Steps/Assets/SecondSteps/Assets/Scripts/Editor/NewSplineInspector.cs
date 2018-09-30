using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SplineMono))]
public class NewSplineInspector : Editor
{

    private Quaternion handleRotation;
    private Transform handleTransform;
    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;
    private const float directionScale = 1.5f;


    private SplineMono splineMono;
    
    private Node selectedNode;
    
    public override void OnInspectorGUI()
    {//movimiento de los inspector en la ventana Scene 

        splineMono = target as SplineMono;
        if (selectedNode != null)
        {
            //DrawSelectedPointInspector();
        }
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RegisterCompleteObjectUndo(splineMono, "Add Curve");
            splineMono.AddFollowingDefaultCurve();
            EditorUtility.SetDirty(splineMono);
        }
    }


    private void OnSceneGUI()
    {//movimiento del mouse en la ventana Scene 

        splineMono = target as SplineMono;

        if (splineMono.Points != null && splineMono.Points.Count > 0)
        {
            handleTransform = splineMono.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            DrawHandles();
            CheckNodeMovement(selectedNode);
            DrawCurves();
            DrawDirections();
        }
        else
        {
           //StartNewSpline();
        }

    }

    private void StartNewSpline()
    {
        splineMono.Points.Clear();
        splineMono.Curves.Clear();
        splineMono.AddFollowingDefaultCurve();

    }


    private void DrawSelectedPointInspector()
    {
        /*
        if (selectedNode == null) return;

        EditorGUI.BeginChangeCheck();
        Vector3 updatedPosition = EditorGUILayout.Vector3Field("Selected Point Position", selectedNode.position);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RegisterCompleteObjectUndo(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            selectedNode.position = updatedPosition;
        }
        */
    }

    private void DrawHandles()
    {
        Handles.color = Color.white;
        foreach (Node node in splineMono.Points)
        {
            float size = HandleUtility.GetHandleSize(node.position);
            if (Handles.Button(node.position, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
            {
                selectedNode = node;
            }
        }
    }

    private void CheckNodeMovement(Node node)
    {
        if (node != null)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 updatedPosition = Handles.DoPositionHandle(node.position, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(splineMono, "Move Point");
                EditorUtility.SetDirty(splineMono);
                //node.position = handleTransform.InverseTransformPoint(updatedPosition);
                node.position = updatedPosition;
            }
        }
    }


    private void DrawCurves()
    {
        Handles.color = Color.grey;
        foreach (Curve curve in splineMono.Curves)
        {
            Handles.DrawBezier(curve.nodoInicio.position,
                curve.nodoFin.position,
                curve.c1.position,
                curve.GetInverseDirection(),
                Color.white,
                null,
                2f);
            Handles.DrawLine(curve.nodoInicio.position, curve.c1.position);
            Handles.DrawLine(curve.nodoFin.position, curve.c2.position);
        }
    }

    private void DrawDirections()
    {
        if (splineMono.Points.Count > 0)
        {
            var splineHelper = new SplineHelper();
            Handles.color = Color.green;

            Vector3 point = splineHelper.GetPositionAtTime(splineMono.spline, 0f);
            Handles.DrawLine(point, point + splineHelper.GetOrientationAtTime(splineMono.spline, 0f) * directionScale);
            int steps = splineMono.Breaks * splineMono.Curves.Count;
            for (float t = 0; t <= splineMono.Curves.Count; t += 1 / (float)splineMono.Breaks)
            {
                point = splineHelper.GetPositionAtTime(splineMono.spline, t);
                Handles.DrawLine(point, point + splineHelper.GetOrientationAtTime(splineMono.spline, t) * directionScale);
            }
            /*for (int i = 1; i <= steps; i++)
            {
                point = spline.GetPositionAtTime(i / (float)spline.breaks);
                Handles.DrawLine(point, point + spline.GetOrientationAtTime(i / (float)steps) * directionScale);
            }*/

        }
    }

}
