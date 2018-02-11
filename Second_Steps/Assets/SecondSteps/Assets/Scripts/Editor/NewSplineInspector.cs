using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(NewSpline))]
public class NewSplineInspector : Editor
{

    private Quaternion handleRotation;
    private Transform handleTransform;
    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;
    private const float directionineScale = 1.5f;


    private NewSpline spline;

    private Node selectedNode;

    public override void OnInspectorGUI()
    {//movimiento de los inspector en la ventana Scene 
        
        spline = target as NewSpline;
        if (selectedNode != null)
        {
            DrawSelectedPointInspector();
        }
    }


    private void OnSceneGUI()
    {//movimiento del mouse en la ventana Scene 

        spline = target as NewSpline;

        if (spline.points.Count > 0)
        {
            handleTransform = spline.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

            DrawHandles();
            CheckNodesMovement();
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


    private void DrawSelectedPointInspector()
    {
        if (selectedNode == null) return;
        /*
        //GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 updatedPosition = EditorGUILayout.Vector3Field("Selected Point Position", selectedNode.position);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            selectedNode.position = updatedPosition;
        }*/
    }

    private void DrawHandles()
    {
        Handles.color = Color.white;
        foreach (Node node in spline.points)
        {
            //Handles.Button(node.position, handleRotation, 2f * handleSize, 2f * pickSize, Handles.DotHandleCap);
            CheckNodeMovement(node);
        }
    }

    private void DrawCurves()
    {
        foreach (Curve curve in spline.curves)
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

    private void CheckNodesMovement()
    {
        foreach (Node node in spline.points)
        {
            //CheckNodeMovement(node);
        }
    }
    private void CheckNodeMovement(Node node)
    {
        float size = HandleUtility.GetHandleSize(node.position);
        if (Handles.Button(node.position, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
        {
            selectedNode = node;
            //Repaint();
        }
        if (selectedNode == node)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 updatedPosition = Handles.DoPositionHandle(node.position, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                node.position = handleTransform.InverseTransformPoint(updatedPosition);
            }
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
