using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Extruder))]
public class ExtruderEditor : Editor
{
    private const int QUAD_SIZE = 10;
    private Color CURVE_COLOR = new Color(0.8f, 0.8f, 0.8f);

    private Extruder extruder;
    private Vertex selection = null;

    private void OnEnable() {
        extruder = (Extruder)target;
    }

    void OnSceneGUI()
    {
        if (!extruder.spline.Started()) return;
        //extruder.Generate();//ESTO LO DIBUJA BIEN
        
        Event e = Event.current;
        if (e.type == EventType.MouseDown) {
            Undo.RegisterCompleteObjectUndo(extruder, "change extruded shape");
        }

        Vector3 splineStartTangent = extruder.spline.GetOrientationAtTime(0);
        Vector3 splineStart = extruder.spline.GetPositionAtTime(0);
        Quaternion q = CubicBezierCurve.GetRotationFromTangent(splineStartTangent);

        foreach (Vertex v in extruder.ShapeVertices) {
            // we create point and normal relative to the spline start where the shape is drawn
            Vector3 point = extruder.transform.TransformPoint(q * v.point + splineStart);
            Vector3 normal = extruder.transform.TransformPoint(q * (v.point + v.normal) + splineStart);

            if (v == selection) {
                // draw the handles for selected vertex position and normal
                float size = HandleUtility.GetHandleSize(point) * 0.3f;
                float snap = 0.1f;

                // create a handle for the vertex position
                Vector3 movedPoint = Handles.Slider2D(0, point, splineStartTangent, Vector3.right, Vector3.up, size, Handles.CircleHandleCap, new Vector2(snap, snap));
                if(movedPoint != point) {
                    // position has been moved
                    Vector2 newVertexPoint = Quaternion.Inverse(q) * (extruder.transform.InverseTransformPoint(movedPoint) - splineStart);

                    v.point = newVertexPoint;
                    // normal must be updated if point has been moved
                    normal = extruder.transform.TransformPoint(q * (v.point + v.normal) + splineStart);

                    extruder.Generate();
                } else {
                    // vertex position handle hasn't been moved
                    // create a handle for normal
                    Vector3 movedNormal = Handles.Slider2D(normal, splineStartTangent, Vector3.right, Vector3.up, size, Handles.CircleHandleCap, snap);
                    if(movedNormal != normal) {
                        // normal has been moved
                        v.normal = (Vector2)(Quaternion.Inverse(q) * (extruder.transform.InverseTransformPoint(movedNormal) - splineStart)) - v.point;
                        extruder.Generate();
                    }
                }

                Handles.BeginGUI();
                DrawQuad(HandleUtility.WorldToGUIPoint(point), CURVE_COLOR);
                DrawQuad(HandleUtility.WorldToGUIPoint(normal), Color.red);
                Handles.EndGUI();
            } else {
                // we draw a button to allow selection of the vertex
                Handles.BeginGUI();
                Vector2 p = HandleUtility.WorldToGUIPoint(point);
                if (GUI.Button(new Rect(p - new Vector2(QUAD_SIZE / 2, QUAD_SIZE / 2), new Vector2(QUAD_SIZE, QUAD_SIZE)), GUIContent.none)) {
                    selection = v;
                }
                Handles.EndGUI();
            }

            // draw an arrow from the vertex location to the normal
            Handles.color = Color.red;
            Handles.DrawLine(point, normal);

            // draw a line between that vertex and the next one
            int index = extruder.ShapeVertices.IndexOf(v);
            int nextIndex = index == extruder.ShapeVertices.Count - 1 ? 0 : index + 1;
            Vertex next = extruder.ShapeVertices[nextIndex];
            Handles.color = CURVE_COLOR;
            Vector3 vAtSplineEnd = extruder.transform.TransformPoint(q * next.point + splineStart);
            Handles.DrawLine(point, vAtSplineEnd);
            
        }

    }

    public override void OnInspectorGUI() {
        
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

}
