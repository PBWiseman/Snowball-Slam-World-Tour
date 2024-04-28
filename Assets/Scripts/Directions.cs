using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Directions
{
    
    private static float angleModifer = 1/Mathf.Sqrt(2);
    public static List<Vector3> directions = new()
    {
        Vector3.forward,
        Vector3.back,
        Vector3.right,
        Vector3.left,
       new Vector3(angleModifer,0,angleModifer),// (Vector3.forward + Vector3.right),
       new Vector3(-angleModifer,0,angleModifer),// (Vector3.forward + Vector3.right),
       new Vector3(angleModifer,0,-angleModifer),// (Vector3.forward + Vector3.right),
       new Vector3(-angleModifer,0,-angleModifer),// (Vector3.forward + Vector3.right),

    };
}