using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracerDebugger : MonoBehaviour 
{
    public List<Vector3> traceList;
    public List<Vector3> contactList;
    public List<Vector3> normal;
    public List<Vector3> reflect;

    public static BulletTracerDebugger Instance;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        traceList = new List<Vector3>();
        contactList = new List<Vector3>();
        normal = new List<Vector3>();
        reflect = new List<Vector3>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (traceList.Count == 0 ) return;

        for (int i = 0; i < traceList.Count; i++)
        {
            DrawDir(contactList[i], traceList[i]);
            //Gizmos.DrawLine(traceList[i], traceList[i + 1]);
        }

        Gizmos.color = Color.blue;
        for (int i = 0;i < contactList.Count;i++) {
            DrawDir(contactList[i], normal[i]);
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < reflect.Count; i++)
        {
            DrawDir(contactList[i], reflect[i]);            
        }
    }

    private void DrawDir(Vector3 pos, Vector3 normal){
        Gizmos.DrawLine (pos, pos + normal);
    }


    public void Clear(){
        traceList.Clear();
        contactList.Clear();
        reflect.Clear();
        normal.Clear();
    }

}
