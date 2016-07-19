﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Editor.Tiled
{
  [XmlRoot(ElementName = "image")]
  public class Image
  {
    [XmlAttribute(AttributeName = "source")]
    public string Source { get; set; }
    [XmlAttribute(AttributeName = "width")]
    public string Width { get; set; }
    [XmlAttribute(AttributeName = "height")]
    public string Height { get; set; }
  }

  [XmlRoot(ElementName = "tileset")]
  public class Tileset
  {
    [XmlElement(ElementName = "image")]
    public Image Image { get; set; }
    [XmlAttribute(AttributeName = "firstgid")]
    public string Firstgid { get; set; }
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }
    [XmlAttribute(AttributeName = "tilewidth")]
    public string Tilewidth { get; set; }
    [XmlAttribute(AttributeName = "tileheight")]
    public string Tileheight { get; set; }
    [XmlAttribute(AttributeName = "tilecount")]
    public string Tilecount { get; set; }
    [XmlAttribute(AttributeName = "columns")]
    public string Columns { get; set; }
  }

  [XmlRoot(ElementName = "data")]
  public class Data
  {
    [XmlAttribute(AttributeName = "encoding")]
    public string Encoding { get; set; }
    [XmlText]
    public string Text { get; set; }
  }

  [XmlRoot(ElementName = "layer")]
  public class Layer
  {
    [XmlElement(ElementName = "data")]
    public Data Data { get; set; }
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }
    [XmlAttribute(AttributeName = "width")]
    public string Width { get; set; }
    [XmlAttribute(AttributeName = "height")]
    public string Height { get; set; }
    [XmlAttribute(AttributeName = "offsetx")]
    public string Offsetx { get; set; }
    [XmlAttribute(AttributeName = "offsety")]
    public string Offsety { get; set; }
  }

  [XmlRoot(ElementName = "object")]
  public class Object
  {
    [XmlAttribute(AttributeName = "id")]
    public string Id { get; set; }
    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }
    [XmlAttribute(AttributeName = "x")]
    public string X { get; set; }
    [XmlAttribute(AttributeName = "y")]
    public string Y { get; set; }
    [XmlAttribute(AttributeName = "width")]
    public string Width { get; set; }
    [XmlAttribute(AttributeName = "height")]
    public string Height { get; set; }
  }

  [XmlRoot(ElementName = "objectgroup")]
  public class Objectgroup
  {
    [XmlElement(ElementName = "object")]
    public List<Object> Object { get; set; }
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }
  }

  [XmlRoot(ElementName = "map")]
  public class Map
  {
    [XmlElement(ElementName = "tileset")]
    public List<Tileset> Tileset { get; set; }
    [XmlElement(ElementName = "layer")]
    public List<Layer> Layer { get; set; }
    [XmlElement(ElementName = "objectgroup")]
    public Objectgroup Objectgroup { get; set; }
    [XmlAttribute(AttributeName = "version")]
    public string Version { get; set; }
    [XmlAttribute(AttributeName = "orientation")]
    public string Orientation { get; set; }
    [XmlAttribute(AttributeName = "renderorder")]
    public string Renderorder { get; set; }
    [XmlAttribute(AttributeName = "width")]
    public int Width { get; set; }
    [XmlAttribute(AttributeName = "height")]
    public int Height { get; set; }
    [XmlAttribute(AttributeName = "tilewidth")]
    public int Tilewidth { get; set; }
    [XmlAttribute(AttributeName = "tileheight")]
    public int Tileheight { get; set; }
    [XmlAttribute(AttributeName = "nextobjectid")]
    public string Nextobjectid { get; set; }
  }
}