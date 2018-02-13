using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {
    //usa el mesher y el spline

    [SerializeField]
    private NewSpline spline;
    public List<Vertex> ShapeVertices = new List<Vertex>();
    public Shape shape;

    private MeshFilter mf;

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        spline = GetComponent<NewSpline>();
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
        spline.AddDefaultCurve();
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