using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LevelFramework
{
    public struct Level
    {
        public string Name;
        public string Path;
        public DirectoryInfo directoryInfo;
        public int SizeX;
        public int SizeY;
        List<Tile> Tiles;
    }

    public struct Tile
    {
        //public byte[] Sprite;
        public int SpriteID;
        public int PosX;
        public int PosY;
        public bool HasCollision;
    }
}
