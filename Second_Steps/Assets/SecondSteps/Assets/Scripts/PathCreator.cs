using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour {
    //usa el mesher y el spline

    public AguSpline spline;
    public List<Vertex> ShapeVertices = new List<Vertex>();
    public Shape shape;

    private MeshFilter mf;

    private void Start()
    {
        Debug.Log("PathCreator Start");
        spline = GetComponent<AguSpline>();
        StartNewSpline();

        mf = GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }

        shape = GetFirstShape();
    }

    private void StartNewSpline()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        Vector3 dir = new Vector3(0, 0, 0);
        Node node = new Node(pos, dir);
        spline.AddNode(node);
        pos = new Vector3(0, 0, 0);
        dir = new Vector3(0, 0, 0);
        node = new Node(pos, dir);
        spline.AddNode(node);
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