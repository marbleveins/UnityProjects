using UnityEngine;
using UnityEditor;
using System;

public class SplineHelper : ScriptableObject
{

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

    public SplineClass MakeNewDefaultSpline(Vector3 startingPosition)
    {
        var spline = new SplineClass();
        spline.AddCurve(GetANewFirstCurve(startingPosition));
        return spline;
    }

    public Curve GetFollowingDefaultCurve(SplineMono splineMono)
    {
        if (splineMono.Points.Count == 0)
        {
            throw new Exception("Error - intentando agregar una curva pero no fue inizializada");
        }

        Node inicio, fin, c1, c2;
        Vector3 c1pos, c1dir, c2pos, c2dir;
        float curveLength = 10;

        inicio = splineMono.Points[splineMono.Points.Count - 1];


        Vector3 tailOffset = new Vector3(inicio.position.x + curveLength, inicio.position.y + curveLength, inicio.position.z + curveLength);
        fin = new Node(tailOffset, inicio.direction);

        Vector3 prevC1pos = splineMono.Points[splineMono.Points.Count - 3].position;
        Vector3 prevC2pos = splineMono.Points[splineMono.Points.Count - 2].position;
        c1pos = new Vector3(prevC1pos.x + curveLength, prevC1pos.y + curveLength, prevC1pos.z + curveLength);
        c1dir = inicio.direction;
        c2pos = new Vector3(prevC2pos.x + curveLength, prevC2pos.y + curveLength, prevC2pos.z + curveLength);
        c2dir = inicio.direction * -1;



        //add
        c1 = new Node(c1pos, c1dir);
        c2 = new Node(c2pos, c2dir);

        return new Curve(inicio, c1, c2, fin);
    }

    public void AddFollowingDefaultCurve(SplineMono splineMono)
    {
        if (splineMono.Points != null && splineMono.Points.Count == 0)
        {
            splineMono.AddCurve(GetANewFirstCurve(splineMono.InitialPosition));
        }
        else
        {
            splineMono.AddCurve(GetFollowingDefaultCurve(splineMono));
        }
    }

    public Vector3 GetPositionAtTime(SplineClass spline, float t)
    {
        return spline.GetPositionAtTime(t);
    }

    public Vector3 GetOrientationAtTime(SplineClass spline, float t)//GetTangentAlongSpline
    {
        return spline.GetOrientationAtTime(t);
    }
}