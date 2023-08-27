using _Scripts.Controller;
using _Scripts.Enemies;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BasicEnemy))]
public class KillEnemyEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BasicEnemy enemy = (BasicEnemy)target;

        if (GUILayout.Button("Kill"))
        {
            enemy.Die();
        }
    }
}