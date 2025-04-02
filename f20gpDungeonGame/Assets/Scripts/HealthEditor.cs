using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Health))]
public class HealthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Health health = (Health)target;

        if (GUILayout.Button("Kill (Debug)"))
        {
            health.TakeDamage(health.maxHealth);
        }
    }
}
