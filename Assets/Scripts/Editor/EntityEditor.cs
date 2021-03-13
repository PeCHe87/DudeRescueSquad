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

            //Handles.color = Color.green;

            // Draws area of detection
            //Handles.DrawWireArc(originPosition, Vector3.up, Vector3.forward, 360, entity.Data.RadiusDetection);
            
            Handles.color = Color.red;
            
            // Draws area of attack
            Handles.DrawWireArc(originPosition, Vector3.up, Vector3.forward, 360, entity.Weapon.AttackAreaRadius);

            Handles.color = Color.yellow;

            Handles.DrawLine(originPosition, originPosition + entity.transform.forward * entity.Weapon.AttackAreaRadius);
        }
    }
}