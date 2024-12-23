using UnityEngine;
using UnityEditor;

namespace Creotly_Studios
{
    [CustomEditor(typeof(EnemyDetectionScript))]
    public class DetectionScriptEditor : Editor
    {
        private void OnSceneGUI()
        {
            EnemyDetectionScript fov = (EnemyDetectionScript)target;
            Handles.color = Color.white;
            Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
            Vector3 viewAngleA = fov.DirectionFromAngle(-fov.viewAngle / 2, false);
            Vector3 viewAngleB = fov.DirectionFromAngle(fov.viewAngle / 2, false);

            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);
        }
    }
}
