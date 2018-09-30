﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {
    //usa el mesher y el spline


    [SerializeField]
    private SplineMono SplineMono { get; set; }
    private SplineClass splineClass;

    private Extruder extruder;
    private List<Vertex> ShapeVertices = new List<Vertex>();
    public Shape shape;

    private MeshFilter mf;

    private void OnEnable()
    {
        
    }

    private void Start()
    {

        SplineMono = GetComponent<SplineMono>();
        StartNewDefaultSpline();
        SplineMono.SetSpline(splineClass);
        extruder = GetComponent<Extruder>();

        mf = GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }

        FormShapeVertices();
        shape = GetFirstShape();
        extruder.Generate();
    }

    private void StartNewDefaultSpline()
    {
        splineClass = SplineMono.MakeNewDefaultSpline();
    }

    private Shape GetFirstShape()
    {
        Shape shape = new Shape(7);
        return shape;
    }

    private void FormShapeVertices()
    {
        ShapeVertices.Clear();

        
        ShapeVertices.Add(new Vertex(new Vector2(0, 0f), new Vector2(0, -1), 0));

        ShapeVertices.Add(new Vertex(new Vector2(-1, 0.5f), new Vector2(-1, -1), 0.66f));
        ShapeVertices.Add(new Vertex(new Vector2(-1.5f, 1.5f), new Vector2(-1, 0), 0.33f));
        ShapeVertices.Add(new Vertex(new Vector2(-1, 2.5f), new Vector2(-1, 1.5f), 0.33f));

        ShapeVertices.Add(new Vertex(new Vector2(1, 2.5f), new Vector2(1, 1.5f), 0));
        ShapeVertices.Add(new Vertex(new Vector2(1.5f, 1.5f), new Vector2(1, 0), 0.66f));
        ShapeVertices.Add(new Vertex(new Vector2(1, 0.5f), new Vector2(1, -1), 0.33f));


    }

    public void SetSpline(SplineMono newSpline)
    {
        this.SplineMono = newSpline;
    }
}