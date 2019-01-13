using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LevelFramework
{
    [Serializable]
    public struct Level
    {
        ///<summary>Name of Level</summary>
        public string Name;
        ///<summary>X Size of Level</summary>
        public int SizeX;
        ///<summary>Y Size of Level</summary>
        public int SizeY;
        ///<summary>Layers in Game</summary>
        public List<Layer> Layer;

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public struct Layer
    {
        ///<summary>Layer Order (first layer, second layer..)</summary>
        public int ZOrder;
        ///<summary>Tiles in Layer</summary>
        public List<Tile> Tiles;
    }

    [Serializable]
    public struct Tile
    {
        ///<summary>Sprite ID of Tile</summary>
        public string SpriteID;
        ///<summary>X Position of Tile</summary>
        public int PosX;
        ///<summary>Y Position of Tile</summary>
        public int PosY;
        ///<summary>Colision of Tile</summary>
        public bool HasCollision;
        ///<summary>Commentary to this Tile</summary>
        public string Commentary;
        ///<summary>Tag to this Tile</summary>
        public string Tag;

        public override string ToString()
        {
            return (PosX + '|' + PosY).ToString();
        }
    }
}
