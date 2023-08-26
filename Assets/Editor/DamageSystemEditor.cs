using _Scripts.Controller;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MechaController))]
public class DamageSystemEditor : UnityEditor.Editor
{
    private int damageAmount = 10;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MechaController damageSystem = (MechaController)target;
        damageAmount = EditorGUILayout.IntField("Damage Amount:", damageAmount);

        if (GUILayout.Button("Apply Damage"))
        {
            damageSystem.DamagePart(damageAmount);
        }
    }
}