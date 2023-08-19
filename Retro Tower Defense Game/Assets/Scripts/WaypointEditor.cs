using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Waypoints))]
public class WaypointEditor : Editor
{
    // Start is called before the first frame update
    Waypoints Waypoints => target as Waypoints;
    private void OnSceneGUI()
    {
        Handles.color = Color.green;
        for (int i = 0; i < Waypoints.Points.Length; i++)
        {
            EditorGUI.BeginChangeCheck();


            //create handles
            Vector3 currentWaypointPoint = Waypoints.CurrentPosition + Waypoints.Points[i];
            Vector3 newWaypointPoint = Handles.FreeMoveHandle(currentWaypointPoint, Quaternion.identity, 0.7f, new Vector3(0.3f, 0.3f, 0.3f), Handles.SphereHandleCap);

            //Labels
            Vector3 allignment = Vector3.down * 0.3f + Vector3.right * 0.3f;
            Handles.Label(Waypoints.CurrentPosition + Waypoints.Points[i] + allignment, $"{i + 1}");

            EditorGUI.EndChangeCheck();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Waypoint Position Handle");
                Waypoints.Points[i] = newWaypointPoint - Waypoints.CurrentPosition;
            }
        }

    }
}
