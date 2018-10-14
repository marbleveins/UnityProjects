using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CommonHelperForNow
{


    public static List<Vertex> GetDefaultShapeVertices()
    {
        List<Vertex> vertices = new List<Vertex>
        {
            new Vertex(new Vector2(0, -1f), new Vector2(0, -1), 0),

            new Vertex(new Vector2(-1, -0.5f), new Vector2(-1, -1), 0.66f),
            new Vertex(new Vector2(-1.5f, 0.5f), new Vector2(-1, 0), 0.33f),
            new Vertex(new Vector2(-1, 1.5f), new Vector2(-1, 1.5f), 0.33f),

            new Vertex(new Vector2(1, 1.5f), new Vector2(1, 1.5f), 0),
            new Vertex(new Vector2(1.5f, 0.5f), new Vector2(1, 0), 0.66f),
            new Vertex(new Vector2(1, -0.5f), new Vector2(1, -1), 0.33f)
        };

        return vertices;
    }






}