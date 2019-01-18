using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using LevelFramework;

namespace LevelEditor
{
    public class LevelManager
    {
        public Level LoadLevel(string _path)
        {
            StreamReader reader = new StreamReader(_path);

            BinaryFormatter formatter = new BinaryFormatter();
            Level lvl = (Level)formatter.Deserialize(reader.BaseStream);
            reader.Close();
            return lvl;
        }

        public void SaveLevel(string _path, Level _level)
        {
            StreamWriter writer = new StreamWriter(_path);

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(writer.BaseStream, _level);
            writer.Close();
        }

        public Level LoadLevelXML(string _path)
        {
            StreamReader reader = new StreamReader(_path);
            XmlSerializer serializer = new XmlSerializer(typeof(Level));
            Level lvl = (Level)serializer.Deserialize(reader.BaseStream);
            reader.Close();
            return lvl;
        }

        public void SaveLevelXML(string _path, Level _level)
        {
            StreamWriter writer = new StreamWriter(_path);
            XmlSerializer serializer = new XmlSerializer(typeof(Level));

            serializer.Serialize(writer.BaseStream, _level);
            writer.Close();
        }
    }
}
