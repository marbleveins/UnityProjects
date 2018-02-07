using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AguSpline : MonoBehaviour
{

    [SerializeField]
    private ControlPoint[] controlPoints;
    public List<Node> nodes = new List<Node>();
    public List<Curve> curves = new List<Curve>();

    public int ControlPointsCount { get { return controlPoints.Length; } }

    public float Length;


    private void CalculateLength()
    {
        Length = 0;
        foreach (var curve in curves)
        {
            Length += curve.Length;
        }
    }
    public Vector3 GetLocationAlongSpline(float t)
    {
        int index = GetNodeIndexForTime(t);
        return curves[index].GetLocation(t - index);
    }
    public Vector3 GetLocationAlongSplineAtDistance(float d)
    {
        if (d < 0 || d > Length)
            throw new ArgumentException(string.Format("Distance must be between 0 and spline length ({0}). Given distance was {1}.", Length, d));
        foreach (Curve curve in curves)
        {
            if (d > curve.Length)
            {
                d -= curve.Length;
            }
            else
            {
                return curve.GetLocationAtDistance(d);
            }
        }
        throw new Exception("Something went wrong with GetLocationAlongSplineAtDistance");
    }
    public Vector3 GetOrientationAtTime(float t)//GetTangentAlongSpline
    {
        int index = GetNodeIndexForTime(t);
        return curves[index].GetTangent(t - index);
    }
    public Vector3 GetOrientationAtDistance(float d)//GetTangentAlongSplineAtDistance
    {
        if (d < 0 || d > Length)
            throw new ArgumentException(string.Format("Distance must be between 0 and spline length ({0}). Given distance was {1}.", Length, d));
        foreach (Curve curve in curves)
        {
            if (d > curve.Length)
            {
                d -= curve.Length;
            }
            else
            {
                return curve.GetTangentAtDistance(d);
            }
        }
        throw new Exception("Something went wrong with GetTangentAlongSplineAtDistance");
    }

    private int GetNodeIndexForTime(float t)
    {
        if (t < 0 || t > nodes.Count - 1)
        {
            throw new ArgumentException(string.Format("Time must be between 0 and last node index ({0}). Given time was {1}.", nodes.Count - 1, t));
        }
        int res = Mathf.FloorToInt(t);
        if (res == nodes.Count - 1)
            res--;
        return res;
    }

    public void AddNode(Node node)
    {
        nodes.Add(node);
        if (nodes.Count != 1)
        {
            Node previousNode = nodes[nodes.IndexOf(node) - 1];
            Curve curve = new Curve(previousNode, node);
            //curve.Changed.AddListener(() => UpdateAfterCurveChanged());
            curves.Add(curve);
        }
        //RaiseNodeCountChanged();
        //UpdateAfterCurveChanged();
    }

    public void InsertNode(int index, Node node)
    {
        if (index == 0)
            throw new Exception("Can't insert a node at index 0");

        Node previousNode = nodes[index - 1];
        Node nextNode = nodes[index];

        nodes.Insert(index, node);

        curves[index - 1].ConnectEnd(node);

        Curve curve = new Curve(node, nextNode);
        //curve.Changed.AddListener(() => UpdateAfterCurveChanged());
        curves.Insert(index, curve);
        //RaiseNodeCountChanged();
        //UpdateAfterCurveChanged();
    }

    public void RemoveNode(Node node)
    {
        int index = nodes.IndexOf(node);

        if (nodes.Count <= 2)
        {
            throw new Exception("Can't remove the node because a spline needs at least 2 nodes.");
        }

        Curve toRemove = index == nodes.Count - 1 ? curves[index - 1] : curves[index];
        if (index != 0 && index != nodes.Count - 1)
        {
            Node nextNode = nodes[index + 1];
            curves[index - 1].ConnectEnd(nextNode);
        }

        nodes.RemoveAt(index);
        //toRemove.Changed.RemoveListener(() => UpdateAfterCurveChanged());
        curves.Remove(toRemove);

        //RaiseNodeCountChanged();
        //UpdateAfterCurveChanged();
    }

    public bool NodeExists(int index)
    {
        return index + 1 >= nodes.Count;
    }


    /*private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        ControlPointMode mode = modes[modeIndex];
        if (mode == ControlPointMode.Free || modeIndex == 0 || modeIndex == modes.Length - 1)
        {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            enforcedIndex = middleIndex + 1;
        }
        else
        {
            fixedIndex = middleIndex + 1;
            enforcedIndex = middleIndex - 1;
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == ControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }*/


        
    public ControlPoint GetControlPoint(int index)
    {
        return controlPoints[index];
    }

    public void SetControlPoint(int index, ControlPoint newCP)
    {
        if (index % 3 == 0)
        {
            Vector3 delta = newCP.position - controlPoints[index].position;
            if (index > 0)
            {
                controlPoints[index - 1].position += delta;
            }
            if (index + 1 < controlPoints.Length)
            {
                controlPoints[index + 1].position += delta;
            }
        }
        controlPoints[index] = newCP;
        //EnforceMode(index);
    }

    public Vector3 GetControlPointLocation(int index)
    {
        return controlPoints[index].position;
    }
    public void SetControlPointLocation(int index, Vector3 loc)
    {
        if (index % 3 == 0)
        {
            Vector3 delta = loc - controlPoints[index].position;
            if (index > 0)
            {
                controlPoints[index - 1].position += delta;
            }
            if (index + 1 < controlPoints.Length)
            {
                controlPoints[index + 1].position += delta;
            }
        }
        controlPoints[index].position = loc;
        //EnforceMode(index);
    }


    public ControlPointMode GetControlPointMode(int index)
    {
        return controlPoints[index].mode;// modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, ControlPointMode mode)
    {
        controlPoints[index].mode = mode;// modes[(index + 1) / 3] = mode;
        //EnforceMode(index);
    }


}
