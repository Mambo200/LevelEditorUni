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
        public string Name;
        public string Path;
        public int SizeX;
        public int SizeY;
        public List<Layer> Layer;
    }

    [Serializable]
    public struct Layer
    {
        public int ZOrder;
        public List<Tile> Tiles;
    }

    [Serializable]
    public struct Tile
    {
        //public byte[] Sprite;
        public int SpriteID;
        public int PosX;
        public int PosY;
        public bool HasCollision;
    }
}
