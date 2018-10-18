using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(SplineMono))]
public class Extruder : MonoBehaviour
{

    private MeshFilter mf;

    public Shape2D shape2d;
    public SplineMono spline;
    public float TextureScale = 1;

    private void Reset()
    {   //Reset is called when the user hits the Reset button in the Inspector's context menu or 
        //when adding the component the first time. This function is only called in editor mode. 
        //Reset is most commonly used to give good default values in the inspector.
        
        
        OnEnable();
    }

    private void Start()
    {
    }

    private void OnValidate()
    {
    }

    private void Update()
    {
    }

    private void OnEnable()
    {
    }

    public bool Initialized()
    {
        return (shape2d != null && shape2d.Vertices != null && shape2d.Vertices.Count > 0);
    }

    public void Generate()
    {
        if (!spline.Initialized()) return;
        List<OrientedPoint> path = spline.GetPath2();
        if (path.Count == 0) return;
                
        int[] triangleIndices = GenerateTriangles(path.Count-1, shape2d.Vertices.Count);
        

        int edgeLoops = path.Count;//por legibilidad. borrar una vez que se entienda
        int allVertices = shape2d.Vertices.Count * path.Count;
        Vector3[] vertices = new Vector3[allVertices];
        Vector3[] normals = new Vector3[allVertices];
        Vector2[] uvs = new Vector2[allVertices];



        int index = 0;
        foreach (OrientedPoint op in path)
        {
            foreach (Vertex v in shape2d.Vertices)
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
        mf = GetComponent<MeshFilter>();
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }

        mf.sharedMesh.Clear();
        mf.sharedMesh.vertices = vertices;
        mf.sharedMesh.normals = normals;
        mf.sharedMesh.uv = uvs;
        mf.sharedMesh.triangles = triangleIndices;
    }

    private int[] GenerateTriangles(int segments, int shapeVertexCount)
    {
        int index = 0;
        int trianglesAmount = shapeVertexCount * 2 * segments;
        //Debug.Log(string.Format("GenerateTriangles=  segments:  {0}, shapeVertices:  {1}, trianglesAmount:  {2}", segments, shapeVertices, trianglesAmount));
        int[] triangleIndices = new int[trianglesAmount * 3];

        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < shapeVertexCount; j++)
            {
                int offset = j == shapeVertexCount - 1 ? -(shapeVertexCount - 1) : 1;
                int a = index + shapeVertexCount;
                int b = index;
                int c = index + offset;
                int d = index + offset + shapeVertexCount;

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