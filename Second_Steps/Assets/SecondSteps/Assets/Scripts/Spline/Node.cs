using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Node
{
    
    public Vector3 position;
    public Vector3 direction;
    public Quaternion rotation;
    public ControlPointMode mode;

    public Node(Vector3 position, Vector3 direction)
    {
        SetPosition(position);
        SetDirection(direction);
        SetRotation(Quaternion.identity);
    }

    public Node(Vector3 position, Vector3 direction, Quaternion rotation)
    {
        SetPosition(position);
        SetDirection(direction);
        SetRotation(rotation);
    }

    public void SetPosition(Vector3 p)
    {
        if (!position.Equals(p))
        {
            position.x = p.x;
            position.y = p.y;
            position.z = p.z;
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

    public void SetRotation(Quaternion r)
    {
        if (!rotation.Equals(r))
        {
            rotation.x = r.x;
            rotation.y = r.y;
            rotation.z = r.z;
            rotation.w = r.w;
        }
    }

    [HideInInspector]
    public UnityEvent Changed = new UnityEvent();
    
}
