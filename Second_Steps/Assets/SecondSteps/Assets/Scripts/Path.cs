using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {
    //usa el mesher y el spline


    [SerializeField]
    private SplineMono SplineMono { get; set; }
    
    private Extruder extruder;

    private MeshFilter mf;

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        StartSpline();
        StartExtruder();
        StartMesh();
        
        extruder.Generate();
    }

    private void StartSpline()
    {
        SplineMono = GetComponent<SplineMono>();
        SplineMono.AddFollowingDefaultCurve();
        SplineMono.AddFollowingDefaultCurve();
        SplineMono.AddFollowingDefaultCurve();
        SplineMono.AddFollowingDefaultCurve();
    }

    private void StartExtruder()
    {
        extruder = GetComponent<Extruder>();
        extruder.shape2d = new Shape2D
        {
            Vertices = CommonHelperForNow.GetDefaultShapeVertices()
        };
    }

    private void StartMesh()
    {
        mf = GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }
    }
}