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
        pos = new Vector3(0, 0, 2);
        dir = new Vector3(0, 0, 0);
        node = new Node(pos, dir);
        spline.AddNode(node);

        ControlPoint cp = new ControlPoint()
        {
            position = new Vector3(0, 2, 0)
        };
        spline.AddControlPoint(cp);
        cp = new ControlPoint()
        {
            position = new Vector3(0, -2, 0)
        };
        spline.AddControlPoint(cp);
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