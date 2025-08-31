using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(godvoice))]
public class Govoicetoggle : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        godvoice script = (godvoice)target;

        if (GUILayout.Button("Force Toggle God Voice"))
        {
            script.ForceToggleGodVoice();
        }
    }
}
