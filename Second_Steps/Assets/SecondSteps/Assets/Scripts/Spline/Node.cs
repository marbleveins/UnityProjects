using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Node
{
    
    public Vector3 position;
    public Vector3 direction;

    public Node(Vector3 position, Vector3 direction)
    {
        SetPosition(position);
        SetDirection(direction);
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

    [HideInInspector]
    public UnityEvent Changed = new UnityEvent();
}
