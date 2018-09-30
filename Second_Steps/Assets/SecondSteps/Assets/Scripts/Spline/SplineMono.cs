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

    public void AddFollowingDefaultCurve()
    {
        var splineHelper = new SplineHelper();
        splineHelper.AddFollowingDefaultCurve(this);
    }

    #endregion
    

    private void Start()
    {
    }

}
