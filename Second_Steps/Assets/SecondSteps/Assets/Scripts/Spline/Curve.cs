using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Curve
{
    private const int STEP_COUNT = 30;
    private const float T_STEP = 1.0f / STEP_COUNT;

    public Node nodoInicio, nodoFin, c1, c2;

    /// <summary>
    /// Length of the curve in world unit.
    /// </summary>
    public float Length { get; private set; }
    private readonly List<CurveSample> samples = new List<CurveSample>(STEP_COUNT);

    /// <summary>
    /// This event is raised when of of the control points has moved.
    /// </summary>
    public UnityEvent Changed = new UnityEvent();

    /// <summary>
    /// Build a new cubic Bézier curve between two given spline node.
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    public Curve(Node inicio, Node c1, Node c2, Node fin)
    {
        this.nodoInicio = inicio;
        this.c1 = c1;
        this.c2 = c2;
        this.nodoFin = fin;
        //n1.Changed.AddListener(() => ComputePoints());
        //n2.Changed.AddListener(() => ComputePoints());
        //ComputePoints();
    }

    /// <summary>
    /// Change the start node of the curve.
    /// </summary>
    /// <param name="n1"></param>
    public void ConnectStart(Node n1)
    {
        this.nodoInicio.Changed.RemoveListener(() => ComputePoints());
        this.nodoInicio = n1;
        n1.Changed.AddListener(() => ComputePoints());
        ComputePoints();
    }

    /// <summary>
    /// Change the end node of the curve.
    /// </summary>
    /// <param name="n2"></param>
    public void ConnectEnd(Node n2)
    {
        this.nodoFin.Changed.RemoveListener(() => ComputePoints());
        this.nodoFin = n2;
        n2.Changed.AddListener(() => ComputePoints());
        ComputePoints();
    }

    /// <summary>
    /// Convinent method to get the third control point of the curve, as the direction of the end spline node indicates the starting tangent of the next curve.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetInverseDirection()
    {
        return c2.position;
        //return (2 * nodoFin.position) - nodoFin.direction;
    }

    /// <summary>
    /// Returns point on curve at given time. Time must be between 0 and 1.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetLocation(float t)
    {
        if (t < 0 || t > 1)
            throw new ArgumentException("Time must be between 0 and 1. Given time was " + t);
        return Bezier.GetPoint(nodoInicio.position, c1.position, c2.position, nodoFin.position, t);
        /*
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return
            nodoInicio.position * (omt2 * omt) +
            nodoInicio.direction * (3f * omt2 * t) +
            GetInverseDirection() * (3f * omt * t2) +
            nodoFin.position * (t2 * t);
        */
    }

    /// <summary>
    /// Returns tangent of curve at given time. Time must be between 0 and 1.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetTangent(float t)
    {
        if (t < 0 || t > 1)
            throw new ArgumentException("Time must be between 0 and 1. Given time was " + t);
        return Bezier.GetTangent(nodoInicio.position, c1.position, c2.position, nodoFin.position, t);
        /*
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        Vector3 tangent =
            nodoInicio.position * (-omt2) +
            nodoInicio.direction * (3 * omt2 - 2 * omt) +
            GetInverseDirection() * (-3 * t2 + 2 * t) +
            nodoFin.position * (t2);
        return tangent.normalized;
        */
    }

    //Es necesario guardar una lista de CurveSample? no se es mejor tener las 2 listas por separado como el spline?
    private void ComputePoints()
    {
        samples.Clear();
        Length = 0;
        Vector3 previousPosition = GetLocation(0);
        for (float t = 0; t < 1; t += T_STEP)
        {
            CurveSample sample = new CurveSample();
            sample.location = GetLocation(t);
            sample.tangent = GetTangent(t);
            Length += Vector3.Distance(previousPosition, sample.location);
            sample.distance = Length;

            previousPosition = sample.location;
            samples.Add(sample);
        }
        CurveSample lastSample = new CurveSample();
        lastSample.location = GetLocation(1);
        lastSample.tangent = GetTangent(1);
        Length += Vector3.Distance(previousPosition, lastSample.location);
        lastSample.distance = Length;
        samples.Add(lastSample);

        if (Changed != null)
            Changed.Invoke();
    }

    private CurveSample getCurvePointAtDistance(float d)
    {
        if (d < 0 || d > Length)
            throw new ArgumentException("Distance must be positive and less than curve length. Length = " + Length + ", given distance was " + d);

        CurveSample previous = samples[0];
        CurveSample next = null;
        foreach (CurveSample cp in samples)
        {
            if (cp.distance >= d)
            {
                next = cp;
                break;
            }
            previous = cp;
        }
        if (next == null)
        {
            throw new Exception("Can't find curve samples.");
        }
        float t = next == previous ? 0 : (d - previous.distance) / (next.distance - previous.distance);

        CurveSample res = new CurveSample();
        res.distance = d;
        res.location = Vector3.Lerp(previous.location, next.location, t);
        res.tangent = Vector3.Lerp(previous.tangent, next.tangent, t).normalized;
        return res;
    }

    /// <summary>
    /// Returns point on curve at distance. Distance must be between 0 and curve length.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public Vector3 GetLocationAtDistance(float d)
    {
        return getCurvePointAtDistance(d).location;
    }

    /// <summary>
    /// Returns tangent of curve at distance. Distance must be between 0 and curve length.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public Vector3 GetTangentAtDistance(float d)
    {
        return getCurvePointAtDistance(d).tangent;
    }

    private class CurveSample
    {
        public Vector3 location;
        public Vector3 tangent;
        public float distance;
    }

    /// <summary>
    /// Convenient method that returns a quaternion used to rotate an object in the tangent direction, considering Y-axis as up vector.
    /// </summary>
    /// <param name="Tangent"></param>
    /// <returns></returns>
    public static Quaternion GetRotationFromTangent(Vector3 Tangent)
    {
        if (Tangent == Vector3.zero)
            return Quaternion.identity;
        return Quaternion.LookRotation(Tangent, Vector3.Cross(Tangent, Vector3.Cross(Vector3.up, Tangent).normalized));
    }
}