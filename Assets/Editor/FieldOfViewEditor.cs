using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FOV))]

public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FOV fov = (FOV)target;
        Handles.color = Color.white;
        Handles.DrawWireDisc(fov.transform.position, Vector3.forward, fov.chaseRadius);

        Vector3 viewAngle01 = DirectionFromAngle(-fov.transform.eulerAngles.z, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(-fov.transform.eulerAngles.z, fov.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.chaseRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.chaseRadius);


        Handles.color = Color.red;
        Handles.DrawWireDisc(fov.transform.position, Vector3.forward, fov.attackRadius);

        if (fov.chasePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.playerRef.transform.position);
        }
    }

    private Vector2 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
