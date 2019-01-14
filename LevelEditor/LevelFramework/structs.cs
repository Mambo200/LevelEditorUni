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

        public Level(string _name, int _sizeX, int _sizeY, List<Layer> _layer)
        {
            Name = _name;
            SizeX = _sizeX;
            SizeY = _sizeY;
            Layer = _layer;
        }

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

        public Layer(int _zOrder, List<Tile> _tile)
        {
            ZOrder = _zOrder;
            Tiles = _tile;
        }
        
        public Layer(int _zOrder, params Tile[] _tile)
        {
            ZOrder = _zOrder;
            Tiles = _tile.ToList();
        }
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

        public Tile(string _spriteID, int _posX, int _posY, bool _hasCollision, string _commentary, string _tag)
        {
            SpriteID = _spriteID;
            PosX = _posX;
            PosY = _posY;
            HasCollision = _hasCollision;
            Commentary = _commentary;
            Tag = _tag;
        }

        public override string ToString()
        {
            return (PosX.ToString() + '|' + PosY.ToString()).ToString();
        }
    }
}
