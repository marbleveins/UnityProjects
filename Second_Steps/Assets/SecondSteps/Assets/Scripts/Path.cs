using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {
    //usa el mesher y el spline

    
    public SplineMono SplineMono;
    public Extruder extruder;
    public MeshFilter mf;


    private void OnEnable()
    {
    }

    public void Awake()
    {
        Initialize();
    }
    public void Start()
    {
        
    }

    public void Initialize()
    {
        if (Initialized()) { return; }

        StartSpline();
        StartExtruder();

    }
    public bool Initialized()
    {
        SplineMono = GetComponent<SplineMono>();
        extruder = GetComponent<Extruder>();
        return SplineMono.Initialized() && extruder.Initialized();
    }

    private void StartSpline()
    {
        SplineMono = GetComponent<SplineMono>();
        SplineMono.AddFollowingDefaultCurve();
        SplineMono.AddFollowingDefaultCurve();
    }

    private void StartExtruder()
    {
        mf = GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }

        extruder = GetComponent<Extruder>();
        extruder.shape2d = new Shape2D
        {
            Vertices = CommonHelperForNow.GetDefaultShapeVertices()
        };
    }
}