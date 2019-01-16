using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LevelFramework;
using LevelEditor;

public class LevelEditorWindow : EditorWindow
{

    [MenuItem("Window/LevelEditor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LevelEditorWindow));
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Open File"))
        {
            LevelManager lvlManager = new LevelManager();
            Level loadedLevel = lvlManager.LoadLevel(EditorUtility.OpenFilePanel("Level to be loaded...", "", "lvl"));
        }
    }
}
