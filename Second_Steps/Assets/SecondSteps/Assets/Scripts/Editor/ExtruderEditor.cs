using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Extruder))]
public class ExtruderEditor : Editor
{
    private const int QUAD_SIZE = 10;
    private Color CURVE_COLOR = new Color(0.8f, 0.8f, 0.8f);
    private bool mustCreateNewNode = false;
    private SerializedProperty textureScale;
    private SerializedProperty vertices;

    private Extruder extruder;
    private Vertex selection = null;

    private void OnEnable() {
        extruder = (Extruder)target;
        textureScale = serializedObject.FindProperty("TextureScale");
        vertices = serializedObject.FindProperty("ShapeVertices");
    }

    void OnSceneGUI()
    {
        if (extruder.spline == null || extruder.spline.spline == null) return;

        Event e = Event.current;
        if (e.type == EventType.MouseDown) {
            Undo.RegisterCompleteObjectUndo(extruder, "change extruded shape");
            // if control key pressed, we will have to create a new vertex if position is changed
            if (e.alt) {
                mustCreateNewNode = true;
            }
        }
        if (e.type == EventType.MouseUp) {
            mustCreateNewNode = false;
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
                    if (mustCreateNewNode) {
                        // We must create a new node
                        mustCreateNewNode = false;
                        Vertex newVertex = new Vertex(newVertexPoint, v.normal, v.uCoord);
                        int i = extruder.ShapeVertices.IndexOf(v);
                        if(i == extruder.ShapeVertices.Count - 1) {
                            extruder.ShapeVertices.Add(newVertex);
                        } else {
                            extruder.ShapeVertices.Insert(i + 1, newVertex);
                        }
                        selection = newVertex;
                    } else {
                        v.point = newVertexPoint;
                        // normal must be updated if point has been moved
                        normal = extruder.transform.TransformPoint(q * (v.point + v.normal) + splineStart);
                    }
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

    void DrawQuad(Rect rect, Color color) {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(rect, GUIContent.none);
    }

    void DrawQuad(Vector2 position, Color color) {
        DrawQuad(new Rect(position - new Vector2(QUAD_SIZE / 2, QUAD_SIZE / 2), new Vector2(QUAD_SIZE, QUAD_SIZE)), color);
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        // Add vertex hint
        EditorGUILayout.HelpBox("Hold Alt and drag a vertex to create a new one.", MessageType.Info);

        // Delete vertex button
        if (selection == null || extruder.ShapeVertices.Count <= 3) {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Delete selected vertex")) {
            Undo.RegisterCompleteObjectUndo(extruder, "delete vertex");
            extruder.ShapeVertices.Remove(selection);
            selection = null;
            extruder.Generate();
        }
        GUI.enabled = true;

        // Properties
        EditorGUILayout.PropertyField(textureScale, true);

        EditorGUILayout.PropertyField(vertices);
        EditorGUI.indentLevel += 1;
        if (vertices.isExpanded) {
            for (int i = 0; i < vertices.arraySize; i++) {
                EditorGUILayout.PropertyField(vertices.GetArrayElementAtIndex(i), new GUIContent("Vertex " + i), true);
            }
        }
        EditorGUI.indentLevel -= 1;

        serializedObject.ApplyModifiedProperties();
    }

}
