using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SplineMono : MonoBehaviour
{

    public List<Node> Points = new List<Node>();
    public List<Curve> Curves = new List<Curve>();

    public int Breaks = 10;
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
    
    
    #region Public Functions

        public bool Started()
        {
            return (this.Points != null && this.Points.Count > 0);
        }
        public void AddCurve(Curve cu)
        {
            Points.Add(cu.nodoInicio);
            Points.Add(cu.c1);
            Points.Add(cu.c2);
            Points.Add(cu.nodoFin);
            Curves.Add(cu);
        }

        public void AddFollowingDefaultCurve()
        {
            if (!Started())
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
            int curveIndex = GetCurveIndexForTime(t);
            return Curves[curveIndex].GetLocation(t - curveIndex);
        }

        public Vector3 GetOrientationAtTime(float t)//GetTangentAlongSpline
        {
            int index = GetCurveIndexForTime(t);
            return Curves[index].GetTangent(t - index);
        }

        public List<OrientedPoint> GetPath()
        {
            var path = new List<OrientedPoint>();
            for (float t = 0; t < Curves.Count; t += 1 / (float)Breaks)
            {
                Debug.Log(string.Format("t: {0} ", t));
                var point = GetPositionAtTime(t);
                var rotation = Curve.GetRotationFromTangent(GetOrientationAtTime(t));
                path.Add(new OrientedPoint(point, rotation));
            }
            return path;
        }
        public List<OrientedPoint> GetPath2()
        {
            var path = new List<OrientedPoint>();
            float t = 0;
            for (float i = 0; i <= Curves.Count * Breaks; i += 1)
            {
                t = i / Breaks;
                Debug.Log(string.Format("t: {0} ", t));
                var point = GetPositionAtTime(t);
                var rotation = Curve.GetRotationFromTangent(GetOrientationAtTime(t));
                path.Add(new OrientedPoint(point, rotation));
            }
            return path;
        }

    #endregion
    
    #region Private Methods

    private Curve GetANewFirstCurve(Vector3 startingPosition)
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

        private Curve GetFollowingDefaultCurve()
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

        private int GetCurveIndexForTime(float t)
        {
            if (t < 0 || t > Curves.Count)
            {
                throw new ArgumentException(string.Format("Time must be between 0 and last node index ({0}). Given time was {1}.", Curves.Count + 1, t));
            }
            int index = Mathf.FloorToInt(t);
            if (index == Curves.Count)
                index--;// ej;si hay una sola curva, t = 1 tiene que devolver index 0
            return index;
        }

        private int NodeCountOnly
        {
            get { return Points.Count - Curves.Count * 2; }
        }

    #endregion
}
