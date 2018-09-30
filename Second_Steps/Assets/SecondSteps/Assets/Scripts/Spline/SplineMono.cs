using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplineMono : MonoBehaviour
{

    public SplineClass spline;

    public List<Node> Points {
        get { return spline ? spline.points : null; }
        //set { }
    }
    public List<Curve> Curves
    {
        get { return spline.curves; }
        //set { }
    }
    public int Breaks
    {
        get { return spline.breaks; }
        //set { }
    }
    
    public bool breaksFixedOnlyTrue = true;
    public bool relativePosition = true;
    
    public Vector3 InitialPosition
    {
        get
        {
            return (relativePosition) ?
              new Vector3(transform.position.x,
              transform.position.y,
              transform.position.z)
              :
              Vector3.zero;
        }
    }
    

    /// <summary>
    /// Public Functions
    /// </summary>
    #region Public Functions

    public void SetSpline(SplineClass spline)
    {
        this.spline = spline;
    }

    public void AddCurve(Curve cu)
    {
        Points.Add(cu.nodoInicio);
        Points.Add(cu.c1);
        Points.Add(cu.c2);
        Points.Add(cu.nodoFin);
        Curves.Add(cu);
    }

    public List<OrientedPoint> GetPath()
    {
        return spline.GetPath();
    }
	public List<OrientedPoint> GetPath2()
	{
        return spline.GetPath2();
	}

    #endregion


    #region helper

    public Curve GetANewFirstCurve(Vector3 startingPosition)
    {

        Node inicio, fin, c1, c2;
        Vector3 c1pos, c1dir, c2pos, c2dir;
        inicio = new Node(startingPosition, Vector3.back);

        fin = new Node(inicio.position + (Vector3.right * 15), Vector3.back);

        c1pos = inicio.position + (Vector3.back * 10);
        c1dir = Vector3.back;

        c2pos = fin.position + (Vector3.forward * 10);
        c2dir = Vector3.forward;


        c1 = new Node(c1pos, c1dir);
        c2 = new Node(c2pos, c2dir);

        return new Curve(inicio, c1, c2, fin);
    }

    public SplineClass MakeNewDefaultSpline()
    {
        var spline = new SplineClass();
        spline.AddCurve(GetANewFirstCurve(InitialPosition));
        return spline;
    }

    public Curve GetFollowingDefaultCurve()
    {
        if (Points.Count == 0)
        {
            throw new Exception("Error - intentando agregar una curva pero no fue inizializada");
        }

        Node inicio, fin, c1, c2;
        Vector3 c1pos, c1dir, c2pos, c2dir;
        float curveLength = 10;

        inicio = Points[Points.Count - 1];


        Vector3 tailOffset = new Vector3(inicio.position.x + curveLength, inicio.position.y + curveLength, inicio.position.z + curveLength);
        fin = new Node(tailOffset, inicio.direction);

        Vector3 prevC1pos = Points[Points.Count - 3].position;
        Vector3 prevC2pos = Points[Points.Count - 2].position;
        c1pos = new Vector3(prevC1pos.x + curveLength, prevC1pos.y + curveLength, prevC1pos.z + curveLength);
        c1dir = inicio.direction;
        c2pos = new Vector3(prevC2pos.x + curveLength, prevC2pos.y + curveLength, prevC2pos.z + curveLength);
        c2dir = inicio.direction * -1;



        //add
        c1 = new Node(c1pos, c1dir);
        c2 = new Node(c2pos, c2dir);

        return new Curve(inicio, c1, c2, fin);
    }

    public void AddFollowingDefaultCurve()
    {
        if (Points != null && Points.Count == 0)
        {
            AddCurve(GetANewFirstCurve(InitialPosition));
        }
        else
        {
            AddCurve(GetFollowingDefaultCurve());
        }
    }

    public Vector3 GetPositionAtTime(float t)
    {
        return spline.GetPositionAtTime(t);
    }

    public Vector3 GetOrientationAtTime(float t)//GetTangentAlongSpline
    {
        return spline.GetOrientationAtTime(t);
    }

    #endregion

}
