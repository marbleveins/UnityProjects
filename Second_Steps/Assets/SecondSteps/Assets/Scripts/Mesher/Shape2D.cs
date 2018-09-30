using System.Collections.Generic;
using UnityEngine;

public class Shape2D : MonoBehaviour
{
	public Shape2D(bool forTest = false)
	{
		if (forTest) {
			Vertices = GetTestVertices ();
		} else {
			Vertices = new List<Vertex> ();
		}
	}

	public List<Vertex> Vertices = new List<Vertex> ();


	private void Reset()
	{
		//Vertices = Shape2D.GetTestVertices ();
	}

	private List<Vertex> GetTestVertices(){
        List<Vertex> vertices = new List<Vertex>
        {
            new Vertex(new Vector2(0, 0f), new Vector2(0, -1), 0),

            new Vertex(new Vector2(-1, 0.5f), new Vector2(-1, -1), 0.66f),
            new Vertex(new Vector2(-1.5f, 1.5f), new Vector2(-1, 0), 0.33f),
            new Vertex(new Vector2(-1, 2.5f), new Vector2(-1, 1.5f), 0.33f),

            new Vertex(new Vector2(1, 2.5f), new Vector2(1, 1.5f), 0),
            new Vertex(new Vector2(1.5f, 1.5f), new Vector2(1, 0), 0.66f),
            new Vertex(new Vector2(1, 0.5f), new Vector2(1, -1), 0.33f)
        };

        return vertices;
	}
}