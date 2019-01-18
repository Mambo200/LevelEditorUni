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
        // open file dialog
        if (GUILayout.Button("Open File"))
        {
            // button pressed
            // create new level Manager
            LevelManager lvlManager = new LevelManager();
            // load level from open file dialog
            Level loadedLevel = lvlManager.LoadLevel(EditorUtility.OpenFilePanel("Level to be loaded...", "", "lvl"));
            // generate level
            GenerateField(loadedLevel);
        }
    }

    /// <summary>
    /// Generate Level with planes
    /// </summary>
    /// <param name="_level">Level to load</param>
    private void GenerateField(Level _level)
    {        
        // parent object
        GameObject parent = new GameObject("Level " + _level.Name);
        // layer parent object
        GameObject[] parentLayer = new GameObject[3];

        // set parents
        parent.transform.position.Set(0, 0, 0);
        // create empty objects with Name and position
        for (int i = 0; i < parentLayer.GetLength(0); i++)
        {
            parentLayer[i] = new GameObject("Layer" + (i + 1) + " " + _level.Name);
            parentLayer[i].transform.position.Set(0, 0, i * -0.1f);
        }

        // load level Layer and Tiles
        Layer[] allLayerInLevel = _level.Layer.ToArray();
        Tile[,] allTilesInLayer = new Tile[allLayerInLevel.GetLength(0), _level.SizeX * _level.SizeY];

        // get all tiles, copy them in an Array
        for (int i = 0; i < allLayerInLevel.GetLength(0); i++)
        {
            int counter = -1;
            foreach (Tile tile in allLayerInLevel[i].Tiles)
            {
                counter++;
                allTilesInLayer[i, counter] = tile;
            }
        }

        // set shader to transparent
        Shader transparent = Shader.Find("Unlit/Transparent");

        for (int layerCount = 0; layerCount < allTilesInLayer.GetLength(0); layerCount++)
        {
            for (int xy = 0; xy < allTilesInLayer.GetLength(1); xy++)
            {
                // Create new Plane
                GameObject m_Plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

                // get current Tile
                Tile CurrentTile = allTilesInLayer[layerCount, xy];
                // set positions
                int posX = CurrentTile.PosX;
                int posY = CurrentTile.PosY;


                #region Set plane stats
                // disable meshcollider
                m_Plane.GetComponent<MeshCollider>().enabled = false;
                // add box collider
                if (CurrentTile.HasCollision == true)
                {
                    m_Plane.AddComponent<BoxCollider>();
                    m_Plane.GetComponent<BoxCollider>().size = new Vector3(10, 10, 10);
                }

                // get Path of Sprite
                string path = "Sprites/";
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

                // load Texture from resources
                Texture m_Texture = Resources.Load<Texture2D>(path);

                // set texture and shader of plane
                m_Plane.GetComponent<Renderer>().material.mainTexture = m_Texture;
                m_Plane.GetComponent<Renderer>().material.shader = transparent;
                #endregion

                // clone plane to Scene
                Instantiate(m_Plane, new Vector3(posX * 10, -posY * 10, 0), Quaternion.Euler(90.0f, 0.0f, 180.0f), parentLayer[layerCount].transform);

                // destroy original plane
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

    /// <summary>
    /// get chars from string at the beginning
    /// </summary>
    /// <param name="_word">string to cut</param>
    /// <param name="_charCount">how many chars shall be copied</param>
    /// <returns>cutted string</returns>
    private string GetFirstChars(string _word, int _charCount)
    {
        char[] idArray = _word.ToCharArray();
        string toReturn = "";
        for (int i = 0; i < _charCount; i++)
        {
            toReturn += idArray[i].ToString();
        }
        return toReturn;
    }

    /// <summary>
    /// get chars from string at the end
    /// </summary>
    /// <param name="word">string to cut</param>
    /// <param name="_charCount">how many chars shall be copied</param>
    /// <returns>cutted string</returns>
    private string GetLastChars(string word, int _charCount)
    {
        char[] idArray = word.ToCharArray();
        string toReturn = "";
        for (int i = 0; i < _charCount; i++)
        {
            toReturn = idArray[idArray.GetLength(0) - (1 + i)] + toReturn;
        }
        return toReturn;
    }

}
