using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpline : MonoBehaviour
{

    public List<Node> nodes = new List<Node>();
    public List<Curve> curves = new List<Curve>();



    public void AddNewDefaultCurve()
    {
        Node n1, n2, c1, c2;
        if (nodes.Count == 0)
        {
            n1 = new Node(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        }
        else
        {
            n1 = nodes[nodes.Count - 1];
        }
        n2 = new Node(new Vector3(n1.location.x + 10, n1.location.y, n1.location.z), new Vector3(n1.direction.x + 10, n1.direction.y, n1.direction.z));

        c1 = new Node(new Vector3(n1.location.x, n1.location.y + 10, n1.location.z), new Vector3(n1.direction.x, n1.direction.y + 10, n1.direction.z));
        c2 = new Node(new Vector3(n2.location.x, n2.location.y + 10, n2.location.z), new Vector3(n2.direction.x, n2.direction.y + 10, n2.direction.z));

        nodes.Add(n1);
        nodes.Add(c1);
        nodes.Add(c2);
        nodes.Add(n2);

        Curve curve = new Curve(n1, n2);
        curve.c1 = c1;
        curve.c2 = c2;
        curves.Add(curve);

    }
}
