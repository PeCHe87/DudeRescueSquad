using UnityEditor;
using UnityEngine;

namespace DudeResqueSquad
{
    [CustomEditor(typeof(Entity))]
    public class EntityEditor : Editor
    {
        private void OnSceneGUI()
        {
            Entity entity = (Entity)target;

            Vector3 originPosition = entity.transform.position;

            Handles.color = Color.red;

            // Draws area of attack
            Handles.DrawWireArc(originPosition, Vector3.up, Vector3.forward, 360, entity.Data.RadiusAttack);

            Handles.color = Color.yellow;

            Handles.DrawLine(originPosition, originPosition + entity.transform.forward * entity.Data.RadiusAttack);
        }
    }
}