using System.Collections.Generic;
using UnityEngine;

public class Shape
{

    public Shape(int verticesCount)
    {
        this.verticesCount = verticesCount;
        vertices = new Vector3[verticesCount];
        normals = new Vector3[verticesCount];
        uvs = new Vector2[verticesCount];
    }

    public int verticesCount;

    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector2[] uvs;

    public Vector3 GetVertice(int index)
    {
        return vertices[index];
    }
    public Vector3 GetNormal(int index)
    {
        return normals[index];
    }
    public Vector2 GetUv(int index)
    {
        return uvs[index];
    }

    public void SetPoint(int index, Vector3 vertice, Vector3 normal, Vector2 uv)
    {
        if (vertices == null || normals == null || uvs == null)
        {
            Debug.Log("Shape.SetPoint; vertices == null || normals == null || uvs == null || IndexOutOfRange");
            return;
        }
        vertices[index] = vertice;
        normals[index] = normal;
        uvs[index] = uv;
    }


    public static Shape Generate(List<OrientedPoint> path, List<Vertex> vertices, float textureScale)
    {
        int edgeLoops = path.Count;//por legibilidad. borrar una vez que se entienda
        Shape shape = new Shape(vertices.Count * path.Count);


        int index = 0;
        foreach (OrientedPoint op in path)
        {
            foreach (Vertex v in vertices)
            {
                shape.SetPoint(index, op.LocalToWorld(v.point),
                    op.LocalToWorldDirection(v.normal),
                    new Vector2(v.uCoord, path.IndexOf(op) / ((float)edgeLoops) * textureScale));
                index++;
            }
        }

        return shape;
    }
}