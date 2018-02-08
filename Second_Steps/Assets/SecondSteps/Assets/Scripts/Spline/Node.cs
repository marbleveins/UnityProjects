using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Node
{
    
    public Vector3 location;
    public Vector3 direction;
    public ControlPointMode mode;

    public Node(Vector3 position, Vector3 direction)
    {
        SetPosition(position);
        SetDirection(direction);
    }

    public void SetPosition(Vector3 p)
    {
        if (!location.Equals(p))
        {
            location.x = p.x;
            location.y = p.y;
            location.z = p.z;
        }
    }

    public void SetDirection(Vector3 d)
    {
        if (!direction.Equals(d))
        {
            direction.x = d.x;
            direction.y = d.y;
            direction.z = d.z;
        }
    }

    [HideInInspector]
    public UnityEvent Changed = new UnityEvent();
}
