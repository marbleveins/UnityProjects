using System;
using UnityEngine;

[Serializable]
public class Vertex
{
    public Vector2 point;
    public Vector2 normal;
    public float uCoord;

    public Vertex(Vector2 point, Vector2 normal, float uCoord)
    {
        this.point = point;
        this.normal = normal;
        this.uCoord = uCoord;
    }
}