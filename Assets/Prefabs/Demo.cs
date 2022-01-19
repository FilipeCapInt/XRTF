using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AstroForge))]
public class Demo : Editor
{
    public Texture tex;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GUILayout.BeginHorizontal();

        

        if (GUILayout.Button("Cycle"))
        {
            //maxHealth = EditorGUILayout.IntField("Health", maxHealth);
        }

        if (GUILayout.Button(tex))
        {

        }

        if (GUILayout.Button(tex))
        {

        }

        GUILayout.EndHorizontal();
    }
}
