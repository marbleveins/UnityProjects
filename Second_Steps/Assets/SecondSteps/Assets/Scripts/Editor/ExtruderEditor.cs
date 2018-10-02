using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Extruder))]
public class ExtruderEditor : Editor
{
    private const int QUAD_SIZE = 10;
    private Color CURVE_COLOR = new Color(0.8f, 0.8f, 0.8f);

    private Extruder extruder;
    private Vertex selectedV = null;

    private void OnEnable() {
        extruder = (Extruder)target;
    }

    void OnSceneGUI()
    {
        if (!extruder.spline.Started()) return;
        CheckExtruder();
        //extruder.Generate();//ESTO LO DIBUJA BIEN

        Event e = Event.current;
        if (e.type == EventType.MouseDown) {
            Undo.RegisterCompleteObjectUndo(extruder, "change extruded shape");
        }

        FormShape();

    }

    public override void OnInspectorGUI() {
        
    }

    private void CheckExtruder()
    {
        if (extruder == null) return;

        if (!extruder.Shaped())
        {
            extruder.shape2d = GetDefaultShape();
        }
    }

    private Shape2D GetDefaultShape()
    {
        return new Shape2D
        {
            Vertices = CommonHelperForNow.GetDefaultShapeVertices()
        };
    }

    private void DrawQuad(Rect rect, Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(rect, GUIContent.none);
    }

    private void DrawQuad(Vector2 position, Color color)
    {
        DrawQuad(new Rect(position - new Vector2(QUAD_SIZE / 2, QUAD_SIZE / 2), new Vector2(QUAD_SIZE, QUAD_SIZE)), color);
    }

    private void FormShape()
    {
        Vector3 startingTangent = extruder.spline.GetOrientationAtTime(0);
        Vector3 startingPosition = extruder.spline.GetPositionAtTime(0);
        Quaternion q = CubicBezierCurve.GetRotationFromTangent(startingTangent);


        foreach (Vertex v in extruder.shape2d.Vertices)
        {
            // we create point and normal relative to the spline start where the shape is drawn
            Vector3 point = extruder.transform.TransformPoint(q * v.point + startingPosition);
            Vector3 normal = extruder.transform.TransformPoint(q * (v.point + v.normal) + startingPosition);

            if (v == selectedV)
            {
                HandleShapeVertexMovement(point, normal, startingPosition, startingTangent, q, v);
            }
            else
            {
                DrawShapeVertexButton(point, v);
            }

            DrawShapeLines(point, normal, startingPosition, q, v);

        }

    }

    private void HandleShapeVertexMovement(Vector3 point, Vector3 normal, Vector3 startingPosition, Vector3 startingTangent, Quaternion q, Vertex v)
    {
        // draw the handles for selected vertex position and normal
        float size = HandleUtility.GetHandleSize(point) * 0.3f;
        float snap = 0.1f;

        // create a handle for the vertex position
        Vector3 movedPoint = Handles.Slider2D(0, point, startingTangent, Vector3.right, Vector3.up, size, Handles.CircleHandleCap, new Vector2(snap, snap));
        Vector3 movedNormal = Handles.Slider2D(normal, startingTangent, Vector3.right, Vector3.up, size, Handles.CircleHandleCap, snap);
        if (movedPoint != point)
        {
            UpdateShapeVertexPosition(normal, startingPosition, startingTangent, movedPoint, q, v);
        }
        else
        {
            DrawShapeVertexNormalHandle(normal, startingPosition, startingTangent, movedNormal, q, v);
        }
        DrawSelectedShapeVertexSquares(point, normal);
    }
    private void UpdateShapeVertexPosition(Vector3 normal, Vector3 startingPosition, Vector3 startingTangent, Vector3 movedPoint, Quaternion q, Vertex v)
    {
        // position has been moved
        Vector2 newVertexPoint = Quaternion.Inverse(q) * (extruder.transform.InverseTransformPoint(movedPoint) - startingPosition);

        v.point = newVertexPoint;
        // normal must be updated if point has been moved
        normal = extruder.transform.TransformPoint(q * (v.point + v.normal) + startingPosition);

        extruder.Generate();
    }
    private void DrawShapeVertexNormalHandle(Vector3 normal, Vector3 startingPosition, Vector3 startingTangent, Vector3 movedNormal, Quaternion q, Vertex v)
    {
        // vertex position handle hasn't been moved
        // create a handle for normal
        if (movedNormal != normal)
        {
            // normal has been moved
            v.normal = (Vector2)(Quaternion.Inverse(q) * (extruder.transform.InverseTransformPoint(movedNormal) - startingPosition)) - v.point;
            extruder.Generate();
        }
    }

    private void DrawShapeVertexButton(Vector3 point, Vertex v)
    {
        // we draw a button to allow selection of the vertex
        Handles.BeginGUI();
        Vector2 p = HandleUtility.WorldToGUIPoint(point);
        if (GUI.Button(new Rect(p - new Vector2(QUAD_SIZE / 2, QUAD_SIZE / 2), new Vector2(QUAD_SIZE, QUAD_SIZE)), GUIContent.none))
        {
            selectedV = v;
        }
        Handles.EndGUI();
    }
    private void DrawShapeLines(Vector3 point, Vector3 normal, Vector3 startingPosition, Quaternion q, Vertex v)
    {
        // draw an arrow from the vertex location to the normal
        Handles.color = Color.red;
        Handles.DrawLine(point, normal);

        // draw a line between that vertex and the next one
        int index = extruder.shape2d.Vertices.IndexOf(v);
        int nextIndex = index == extruder.shape2d.Vertices.Count - 1 ? 0 : index + 1;
        Vertex next = extruder.shape2d.Vertices[nextIndex];
        Handles.color = CURVE_COLOR;
        Vector3 vAtSplineEnd = extruder.transform.TransformPoint(q * next.point + startingPosition);
        Handles.DrawLine(point, vAtSplineEnd);
    }
    private void DrawSelectedShapeVertexSquares(Vector3 point, Vector3 normal)
    {
        Handles.BeginGUI();
        DrawQuad(HandleUtility.WorldToGUIPoint(point), CURVE_COLOR);
        DrawQuad(HandleUtility.WorldToGUIPoint(normal), Color.red);
        Handles.EndGUI();
    }

}
