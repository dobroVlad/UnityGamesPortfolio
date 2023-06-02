using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LadderSettings))]
public class LadderSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LadderSettings ladder = (LadderSettings)target;
        if(GUILayout.Button("Update size of ladder"))
        {
            ladder.CreateLadder();
        }
    }
}
