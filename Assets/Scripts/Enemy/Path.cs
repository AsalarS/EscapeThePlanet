using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Path : MonoBehaviour
{
    public List<Transform> waypoints;
    [SerializeField]
    private bool alwaysDrawPath;
    [SerializeField]
    private bool drawAsLoop;
    [SerializeField]
    private bool drawNumbers;
    public Color debugColour = Color.white;

    public void OnDrawGizmos()
    {
        if (alwaysDrawPath)
        {
            DrawPath();
        }
    }

    public void DrawPath()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 30;
            labelStyle.normal.textColor = debugColour;
#if UNITY_EDITOR
            if (drawNumbers)
                UnityEditor.Handles.Label(waypoints[i].position, i.ToString(), labelStyle);
#endif
            //Draw Lines Between Points.
            if (i >= 1)
            {
                Gizmos.color = debugColour;
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);

                if (drawAsLoop)
                    Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (alwaysDrawPath)
            return;
        else
            DrawPath();
    }

    public static string GetFileName(string path)
    {
        throw new NotImplementedException();
    }
}
