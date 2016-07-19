using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.Editor
{
  [XmlRoot(ElementName = "nameValueStorePair")]
  public class NameValueStorePair
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName = "value")]
    public string Value { get; set; }
  }

  [XmlRoot(ElementName = "nameValueStore")]
  public class NameValueStore
  {
    [XmlElement(ElementName = "items")]
    public List<NameValueStorePair> Pairs { get; set; }

    public void SetValue(string name, string value)
    {
      var pair = Pairs.FirstOrDefault(p => p.Name == name);

      if (pair != null)
      {
        pair.Value = value;

        return;
      }

      Pairs.Add(new NameValueStorePair { Name = name, Value = value });
    }

    public string GetValue(string name)
    {
      var pair = Pairs.FirstOrDefault(p => p.Name == name);

      if (pair != null)
      {
        return pair.Value;
      }

      return null;
    }

    public void Serialize(string fileName)
    {
      Serialize(Application.persistentDataPath, fileName);
    }

    public void Serialize(string filePath, string fileName)
    {
      var fullPath = Path.Combine(filePath, fileName).Replace("\\", "/");

      if (File.Exists(fullPath))
      {
        File.Delete(fullPath);
      }

      var serializer = new XmlSerializer(typeof(NameValueStore));

      using (var streamWriter = new StreamWriter(fullPath))
      {
        serializer.Serialize(streamWriter, this);
      }
    }

    public static NameValueStore Deserialize(string fileName)
    {
      return Deserialize(Application.persistentDataPath, fileName);
    }

    public static NameValueStore Deserialize(string filePath, string fileName)
    {
      var fullPath = Path.Combine(filePath, fileName).Replace("\\", "/");

      if (!File.Exists(fullPath))
      {
        return new NameValueStore { Pairs = new List<NameValueStorePair>() };
      }

      var serializer = new XmlSerializer(typeof(NameValueStore));

      using (var reader = new StreamReader(fullPath))
      {
        return (NameValueStore)serializer.Deserialize(reader);
      }
    }

  }
}
