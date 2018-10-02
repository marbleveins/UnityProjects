using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(SplineMono))]
public class Extruder : MonoBehaviour
{

    private MeshFilter mf;

    public SplineMono spline;
    public float TextureScale = 1;
    public List<Vertex> ShapeVertices = new List<Vertex>();

    private bool toUpdate = true;

    private void Reset()
    {
        ShapeVertices.Clear();


        ShapeVertices.Add(new Vertex(new Vector2(0, 0f), new Vector2(0, -1), 0));

        ShapeVertices.Add(new Vertex(new Vector2(-1, 0.5f), new Vector2(-1, -1), 0.66f));
        ShapeVertices.Add(new Vertex(new Vector2(-1.5f, 1.5f), new Vector2(-1, 0), 0.33f));
        ShapeVertices.Add(new Vertex(new Vector2(-1, 2.5f), new Vector2(-1, 1.5f), 0.33f));

        ShapeVertices.Add(new Vertex(new Vector2(1, 2.5f), new Vector2(1, 1.5f), 0));
        ShapeVertices.Add(new Vertex(new Vector2(1.5f, 1.5f), new Vector2(1, 0), 0.66f));
        ShapeVertices.Add(new Vertex(new Vector2(1, 0.5f), new Vector2(1, -1), 0.33f));


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
        spline = GetComponent<SplineMono>();
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }
        //spline.NodeCountChanged.AddListener(() => toUpdate = true);
        //spline.CurveChanged.AddListener(() => toUpdate = true);
    }

    private void Update()
    {
        if (toUpdate)
        {
            //Generate();
            toUpdate = false;
        }
    }

    public void Generate()
    {
        List<OrientedPoint> path = spline.GetPath2();
        if (path.Count == 0) return;
        //Shape shape = Shape.Generate(path, ShapeVertices, TextureScale);
        int[] triangleIndices = GenerateTriangles(path.Count-1, ShapeVertices.Count);
        



        int edgeLoops = path.Count;//por legibilidad. borrar una vez que se entienda
        int allVertices = ShapeVertices.Count * path.Count;
        Vector3[] vertices = new Vector3[allVertices];
        Vector3[] normals = new Vector3[allVertices];
        Vector2[] uvs = new Vector2[allVertices];



        int index = 0;
        foreach (OrientedPoint op in path)
        {
            foreach (Vertex v in ShapeVertices)
            {
                vertices[index] = op.LocalToWorld(v.point);
                normals[index] = op.LocalToWorldDirection(v.normal);
                uvs[index] = new Vector2(v.uCoord, path.IndexOf(op) / ((float)edgeLoops) * TextureScale);
                
                index++;
            }
        }
        

        SetMesh(vertices, normals, uvs, triangleIndices);
    }

    private void SetMesh(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangleIndices)
    {
        mf.sharedMesh.Clear();
        mf.sharedMesh.vertices = vertices;
        mf.sharedMesh.normals = normals;
        mf.sharedMesh.uv = uvs;
        mf.sharedMesh.triangles = triangleIndices;
    }

    private int[] GenerateTriangles(int segments, int shapeVertices)
    {
        int index = 0;
        int trianglesAmount = shapeVertices * 2 * segments;
        //Debug.Log(string.Format("GenerateTriangles=  segments:  {0}, shapeVertices:  {1}, trianglesAmount:  {2}", segments, shapeVertices, trianglesAmount));
        int[] triangleIndices = new int[trianglesAmount * 3];

        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < shapeVertices; j++)
            {
                int offset = j == shapeVertices - 1 ? -(shapeVertices - 1) : 1;
                int a = index + shapeVertices;
                int b = index;
                int c = index + offset;
                int d = index + offset + shapeVertices;

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