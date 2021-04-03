using UnityEditor;
using UnityEngine;

namespace DudeResqueSquad
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : Editor
    {
        private void OnSceneGUI()
        {
            FieldOfView fov = (FieldOfView)target;

            Vector3 originPosition = fov.transform.position;

            Handles.color = Color.magenta;

            // Draws area of detection
            Handles.DrawWireArc(originPosition, Vector3.up, Vector3.forward, 360, fov.Radius);

            // Draws the limits of cone view
            Vector3 viewAngleA = fov.DirectionFromAngle(-fov.ViewAngle / 2);
            Vector3 viewAngleB = fov.DirectionFromAngle(fov.ViewAngle / 2);

            Handles.color = fov.ColorVisionConeDebug;
            
            Handles.DrawLine(originPosition, originPosition + viewAngleA * fov.Radius);
            Handles.DrawLine(originPosition, originPosition + viewAngleB * fov.Radius);

            // Show lines towards detected targets if there are some of them
            if (fov.VisibleTargets == null)
                return;

            Handles.color = Color.red;

            int amountOfTargets = fov.VisibleTargets.Count;

            for (int i = 0; i < amountOfTargets; i++)
            {
                if (fov.VisibleTargets[i].Equals(fov.NearestTarget))
                    Handles.color = Color.green;
                else
                    Handles.color = Color.red;

                Handles.DrawLine(originPosition, fov.VisibleTargets[i].position);
            }
        }
    }
}