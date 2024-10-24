using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BulletTracerDebugger 
{
    public static void Draw(Vector3 start, Vector3 end){
        Gizmos.color = Color.green;
        Gizmos.DrawIcon(start, "start", true);
        Gizmos.DrawIcon(end, "end", true);
        Gizmos.DrawLine(start, end);

        Debug.DrawLine(start, end);
    }

    public static void Highlight(){
        Gizmos.color = Color.red;
    }
}
