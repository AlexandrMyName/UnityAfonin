using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SaveLoad 
{
    public static void Save (string Path, LocalSave save)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(LocalSave));
        FileStream fileStream = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write);
        xmlSerializer.Serialize(fileStream, save);
        fileStream.Close();
    }

    public static LocalSave Load (string Path)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(LocalSave));
        FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
        LocalSave localData = (LocalSave)xmlSerializer.Deserialize(fileStream);
        fileStream.Close();
        return localData;
    }
}
