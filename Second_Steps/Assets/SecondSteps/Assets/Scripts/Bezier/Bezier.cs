using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{




    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
        //return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return
            2f * (1f - t) * (p1 - p0) +
            2f * t * (p2 - p1);
    }

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }





    public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        return (p0 * (-omt2) +
                p1 * (3f * omt2 - 2 * omt) +
                p2 * (-3f * t2 + 2 * t) +//3f lo cambié por -3f y dibujó bien. revisar si asi deberia ser o está mal en otro lado
                p3 * t2).normalized;
    }

    public static Vector3 GetPointOtherFormula(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);

        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return p0 * (omt2 * omt) +
                p1 * (3f * omt2 * t) +
                p2 * (3f * omt * t2) +
                p3 * (t2 * t);
    }

    public static Vector3 GetFirstDerivativeOtherFormula(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        return p0 * (-1 * (oneMinusT * oneMinusT)) +
            p1 * (t * (3 * t - 4) + 1) +
            p2 * (-3 * t * t + 2 * t) +
            p3 * t * t;
    }







    public static Vector3 GetNormal3D(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, Vector3 up)
    {
        //https://www.youtube.com/watch?v=o9RK6O2kOKo

        Vector3 tng = GetTangent(p0, p1, p2, p3, t);

        Vector3 biNormal = Vector3.Cross(up, tng).normalized;
        return Vector3.Cross(tng, biNormal);
    }

    public static Quaternion GetDirection3D(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, Vector3 up)
    {
        //https://www.youtube.com/watch?v=o9RK6O2kOKo

        Vector3 tng = GetTangent(p0, p1, p2, p3, t);
        Vector3 nrm = GetNormal3D(p0, p1, p2, p3, t, up);
        return Quaternion.LookRotation(tng, nrm);
    }






    //first

    public static Vector3 CalculateLinearBezierPoint(float t, Vector3 p0, Vector3 p1)
    {
        return p0 + (p1 - p0) * t;
    }

    public static Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        //B(t) = (1 - t)2P0 + 2(1 - t)tP1 + t2P2
        var omt = 1 - t;
        var omt2 = omt * omt;
        var ut2 = 2 * omt * t;
        var tt = t * t;

        return omt2 * p0 + ut2 * p1 + tt * p2;
    }

    public static Vector3 CalculateQubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //B(t) = (1-t)3P0 + 3(1-t)2tP1 + 3(1-t)t2P2 + t3P3
        var omt = 1 - t;
        var omt2 = omt * omt;
        var omt3 = omt * omt * omt;
        var ut2 = 2 * omt * t;
        var tt = t * t;
        var ttt = t * t * t;

        return omt3 * p0 + 3 * omt2 * t * p1 + 3 * omt * ttt * p2 + ttt * p3;
    }




}
