using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewSpline : MonoBehaviour
{

    public List<Node> points = new List<Node>();
    public List<Curve> curves = new List<Curve>();

    //private List<Node> GetPoints()
    //{
    //    return new List<Node>().AddRange()
    //}
    public int breaks = 10;
    public bool breaksFixedOnlyTrue = true;
    public bool relativePosition = true;

    private Vector3 InitialPosition
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


    public void AddDefaultCurve()
    {
        if (points.Count == 0)
        {
            AddCurve(GetANewFirstCurve());
        }
        else
        {
            //throw new Exception("AddCurve(GetNextCurve()) - no deberia");
            AddCurve(GetNextCurve());
        }

    }

    public void AddCurve(Curve cu)
    {
        points.Add(cu.nodoInicio);
        points.Add(cu.c1);
        points.Add(cu.c2);
        points.Add(cu.nodoFin);
        curves.Add(cu);
    }
    public Vector3 GetPositionAtTime(float t)
    {
        int curveIndex = GetCurveIndexForTime(t);
        return curves[curveIndex].GetLocation(t - curveIndex);
    }
    public Vector3 GetOrientationAtTime(float t)//GetTangentAlongSpline
    {
        int index = GetCurveIndexForTime(t);
        return curves[index].GetTangent(t - index);
    }


    #endregion


    /// <summary>
    /// Private Methods
    /// </summary>
    #region Private Methods


    private int GetCurveIndexForTime(float t)
    {
        if (t < 0 || t > curves.Count)
        {
            throw new ArgumentException(string.Format("Time must be between 0 and last node index ({0}). Given time was {1}.", points.Count - 1, t));
        }
        int index = Mathf.FloorToInt(t);
        if (index == curves.Count)
            index--;// ej;si hay una sola curva, t = 1 tiene que devolver index 0
        return index;
    }

    private int NodeCountOnly
    {
        get { return points.Count - curves.Count * 2; }
    }


    private Curve GetANewFirstCurve()
    {

        Node inicio, fin, c1, c2;
        Vector3 c1pos, c1dir, c2pos, c2dir;
        inicio = new Node(InitialPosition, Vector3.back);

        fin = new Node(inicio.position + (Vector3.right * 15), Vector3.back);

        c1pos = inicio.position + (Vector3.back * 10);
        c1dir = Vector3.back;

        c2pos = fin.position + (Vector3.forward * 10);
        c2dir = Vector3.forward;


        c1 = new Node(c1pos, c1dir);
        c2 = new Node(c2pos, c2dir);

        return new Curve(inicio, c1, c2, fin);
    }
    private Curve GetNextCurve()
    {
        if (points.Count == 0)
        {
            throw new Exception("Error - intentando agregar una curva pero no fue inizializada");
        }

        Node inicio, fin, c1, c2;
        Vector3 c1pos, c1dir, c2pos, c2dir;
        float curveLength = 10;

        inicio = points[points.Count - 1];


        Vector3 tailOffset = new Vector3(inicio.position.x + curveLength, inicio.position.y + curveLength, inicio.position.z + curveLength);
        fin = new Node(tailOffset, inicio.direction);

        Vector3 prevC1pos = points[points.Count - 3].position;
        Vector3 prevC2pos = points[points.Count - 2].position;
        c1pos = new Vector3(prevC1pos.x + curveLength, prevC1pos.y + curveLength, prevC1pos.z + curveLength);
        c1dir = inicio.direction;
        c2pos = new Vector3(prevC2pos.x + curveLength, prevC2pos.y + curveLength, prevC2pos.z + curveLength);
        c2dir = inicio.direction * -1;



        //add
        c1 = new Node(c1pos, c1dir);
        c2 = new Node(c2pos, c2dir);

        return new Curve(inicio, c1, c2, fin);
    }

    #endregion

}
