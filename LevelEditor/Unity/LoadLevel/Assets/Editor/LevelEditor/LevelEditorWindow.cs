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
        GameObject[] parentLayer = new GameObject[3];

        for (int i = 0; i < parentLayer.GetLength(0); i++)
        {
            parentLayer[i] = new GameObject("Layer" + (i + 1) + " " + _level.Name);
        }

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

        // set parents
        parent.transform.position.Set(0, 0, 0);

        


        for (int layerCount = 0; layerCount < allTilesInLayer.GetLength(0); layerCount++)
        {
            for (int xy = 0; xy < allTilesInLayer.GetLength(1); xy++)
            {
                GameObject m_Plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

                Tile CurrentTile = allTilesInLayer[layerCount, xy];
                int posX = CurrentTile.PosX;
                int posY = CurrentTile.PosY;


                #region Set stats
                // disable meshcollider
                m_Plane.GetComponent<MeshCollider>().enabled = false;
                // add box collider
                if (CurrentTile.HasCollision == true)
                {
                    m_Plane.AddComponent<BoxCollider>();
                    m_Plane.GetComponent<BoxCollider>().size = new Vector3(10, 10, 10);
                }
                // set texture
                Material m_PlaneMat = m_Plane.GetComponent<Renderer>().sharedMaterial;
                string path = /*Application.dataPath + */"Sprites/";

                string cut = "";
                if (CurrentTile.SpriteID == "0")
                    cut = CurrentTile.SpriteID = "00";
                else
                    cut = GetFirstChars(CurrentTile.SpriteID, 2);

                bool continuePath = true;

                switch (cut)
                {
                    case "A1":
                        path += "Outside_A1/Frames/A1_tile";
                        break;
                    case "A2":
                        path += "Outside_A2/Frames/A2_tile";
                        break;
                    case "A3":
                        path += "Outside_A3/Frames/A3_tile";
                        break;
                    case "A4":
                        path += "Outside_A4/Frames/A4_tile";
                        break;
                    case "A5":
                        path += "Outside_A5/Frames/A5_tile";
                        break;
                    case "B1":
                        path += "Outside_B/Frames/B1_tile";
                        break;
                    case "C1":
                        path += "Outside_C/Frames/C1_tile";
                        break;
                    default:
                        path += "empty";
                        continuePath = false;
                        break;
                }

                if (continuePath == true)
                {
                    path += GetLastChars(CurrentTile.SpriteID, 3);
                }

                /* "Assets\\Sprites\\Outside_A1\\Frames\\" + CurrentTile.SpriteID.ToString();*/



                Texture m_Texture = Resources.Load<Texture2D>(path);

                m_Plane.GetComponent<Renderer>().material.mainTexture = m_Texture;
                //Renderer rend = m_Plane.GetComponent<Renderer>();
                //rend.material.mainTexture = m_Texture;
                #endregion

                Instantiate(m_Plane, new Vector3(posX * 10, -posY * 10, layerCount * -0.1f), Quaternion.Euler(90.0f, 0.0f, 180.0f), parentLayer[layerCount].transform);

                DestroyImmediate(m_Plane);

            }

        }

        // copy layer to parrent and destroy
        for (int i = 0; i < parentLayer.GetLength(0); i++)
        {
            parentLayer[i].transform.position.Set(0, 0, 0);
            Instantiate(parentLayer[i], parent.transform);
            DestroyImmediate(parentLayer[i]);
        }
    }
    private string GetFirstChars(string _id, int _charCount)
    {
        char[] idArray = _id.ToCharArray();
        string toReturn = "";
        for (int i = 0; i < _charCount; i++)
        {
            toReturn += idArray[i].ToString();
        }
        return toReturn;
    }

    private string GetLastChars(string _id, int _charCount)
    {
        /*char[] idArray = _id.ToCharArray();
        string toReturn = "";
        for (int i = 0; i < _charCount; i++)
        {
            toReturn = idArray[idArray.GetLength(0) - _charCount] + toReturn;
        }
        return toReturn;*/

        char[] idArray = _id.ToCharArray();
        string toReturn = "";
        for (int i = 0; i < _charCount; i++)
        {
            toReturn = idArray[idArray.GetLength(0) - (1 + i)] + toReturn;
        }
        return toReturn;
    }

}
