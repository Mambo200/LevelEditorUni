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

        //m_Plane = (GameObject)EditorGUILayout.ObjectField(m_Plane, typeof(GameObject), true);

        if (GUILayout.Button("Open File"))
        {
            LevelManager lvlManager = new LevelManager();
            Level loadedLevel = lvlManager.LoadLevel(EditorUtility.OpenFilePanel("Level to be loaded...", "", "lvl"));
            GenerateField(loadedLevel);
        }
    }

    private void GenerateField(Level _level)
    {        
        // parent
        GameObject parent = new GameObject("Level " + _level.Name);
        Instantiate(parent);
        GameObject m_Plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        Layer[] allLayerInLevel = _level.Layer.ToArray();
        Tile[,] allTilesInLayer = new Tile[allLayerInLevel.GetLength(0), _level.SizeX * _level.SizeY];

        // get all tiles
        for (int i = 0; i < allLayerInLevel.GetLength(0); i++)
        {
            int counter = -1;
            foreach (Tile tile in allLayerInLevel[i].Tiles)
            {
                counter++;
                allTilesInLayer[i, counter] = tile;
            }
        }

        parent.transform.position.Set(0, 0, 0);

        for (int layerCount = 0; layerCount < allTilesInLayer.GetLength(0); layerCount++)
        {
            for (int xy = 0; xy < allTilesInLayer.GetLength(1); xy++)
            {
                Tile CurrentTile = allTilesInLayer[layerCount, xy];
                int posX = CurrentTile.PosX;
                int posY = CurrentTile.PosY;

                GameObject SpawnedTile = Instantiate(m_Plane, new Vector3(posX * 10, -posY * 10, layerCount * .1f), Quaternion.Euler(90.0f, 0.0f, 180.0f), parent.transform);
                //if (CurrentTile.SpriteID != "0")
                //{
                //
                //    Texture texture = Resources.FindObjectsOfTypeAll<Texture>()[0];
                //    SpawnedTile.GetComponent<Material>().SetTexture
                //        (
                //        CurrentTile.SpriteID,
                //        texture
                //        );
                //}
            }
        }
        DestroyImmediate(m_Plane);
    }
}
