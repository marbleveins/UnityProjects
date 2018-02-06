using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Spline))]
public class Extruder : MonoBehaviour
{

    private MeshFilter mf;

    public Spline spline;
    public float TextureScale = 1;
    public List<Vertex> ShapeVertices = new List<Vertex>();

    private bool toUpdate = true;

    private void Reset()
    {
        ShapeVertices.Clear();
        ShapeVertices.Add(new Vertex(new Vector2(0, 0.5f), new Vector2(0, 1), 0));
        ShapeVertices.Add(new Vertex(new Vector2(1, -0.5f), new Vector2(1, -1), 0.33f));
        ShapeVertices.Add(new Vertex(new Vector2(-1, -0.5f), new Vector2(-1, -1), 0.66f));




        ShapeVertices.Add(new Vertex(new Vector2(0, 0f), new Vector2(0, 1), 0));
        ShapeVertices.Add(new Vertex(new Vector2(1, 0.5f), new Vector2(-1, 1), 0.33f));
        ShapeVertices.Add(new Vertex(new Vector2(-1, 0.5f), new Vector2(1, 1), 0.33f));

        ShapeVertices.Add(new Vertex(new Vector2(-1, -0.5f), new Vector2(-1, -1), 0.66f));

        toUpdate = true;
        OnEnable();
    }

    private void OnValidate()
    {
        toUpdate = true;//esto es lo que genera el mesh por frame, depende de esto
    }

    private void OnEnable()
    {
        mf = GetComponent<MeshFilter>();
        spline = GetComponent<Spline>();
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }
        spline.NodeCountChanged.AddListener(() => toUpdate = true);
        spline.CurveChanged.AddListener(() => toUpdate = true);
    }

    private void Update()
    {
        if (toUpdate)
        {
            Generate();
            toUpdate = false;
        }
    }

    private List<OrientedPoint> GetPath()
    {//esto deberia ser Spline.GetList() y que haga esto mismo pero adentro porque el spline se mantiene con varios arrays en vez de 1
        // una vez hecho el spline, moverlo
        var path = new List<OrientedPoint>();
        for (float t = 0; t < spline.nodes.Count - 1; t += 1 / 10.0f)
        {
            var point = spline.GetLocationAlongSpline(t);
            var rotation = CubicBezierCurve.GetRotationFromTangent(spline.GetTangentAlongSpline(t));
            path.Add(new OrientedPoint(point, rotation));
        }
        return path;
    }

    public void Generate()
    {
        List<OrientedPoint> path = GetPath();
        Shape shape = Shape.Generate(path, ShapeVertices, TextureScale);
        int[] triangleIndices = GenerateTriangles(path.Count - 1, shape.verticesCount);

        SetMesh(shape, triangleIndices);
    }

    public void SetMesh(Shape shape, int[] triangleIndices)
    {
        mf.sharedMesh.Clear();
        mf.sharedMesh.vertices = shape.vertices;
        mf.sharedMesh.normals = shape.normals;
        mf.sharedMesh.uv = shape.uvs;
        mf.sharedMesh.triangles = triangleIndices;
    }

    private int[] GenerateTriangles(int segments, int vertices)
    {

        int index = 0;
        int trianglesAmount = vertices * 2 * segments * 3;
        int[] triangleIndices = new int[trianglesAmount];

        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < vertices; j++)
            {
                int offset = j == vertices - 1 ? -(vertices - 1) : 1;
                int a = index + vertices;
                int b = index;
                int c = index + offset;
                int d = index + offset + vertices;

                triangleIndices[index * 6 + 0] = c;
                triangleIndices[index * 6 + 1] = b;
                triangleIndices[index * 6 + 2] = a;
                triangleIndices[index * 6 + 3] = a;
                triangleIndices[index * 6 + 4] = d;
                triangleIndices[index * 6 + 5] = c;

                index++;
            }
        }
        return triangleIndices;

    }

}