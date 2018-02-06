using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour {
    //usa el mesher y el spline

    public Spline spline;
    public List<Vertex> ShapeVertices = new List<Vertex>();
    public Shape shape;

    private MeshFilter mf;

    private void Start()
    {
        mf = GetComponent<MeshFilter>();
        spline = GetComponent<Spline>();
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }

        shape = GetFirstShape();
    }



    private Shape GetFirstShape()
    {
        Shape shape = new Shape(7);
        return shape;
    }

    public void GetNewPath()
    {

    }
}