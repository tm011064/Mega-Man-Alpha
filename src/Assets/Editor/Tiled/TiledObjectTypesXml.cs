using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Editor.Tiled
{
  [XmlRoot(ElementName = "property")]
  public class ObjectProperty
  {
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }
    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }
    [XmlAttribute(AttributeName = "default")]
    public string Default { get; set; }
  }

  [XmlRoot(ElementName = "objecttype")]
  public class Objecttype
  {
    [XmlElement(ElementName = "property")]
    public List<ObjectProperty> Properties { get; set; }
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }
    [XmlAttribute(AttributeName = "color")]
    public string Color { get; set; }
  }

  [XmlRoot(ElementName = "objecttypes")]
  public class Objecttypes
  {
    [XmlElement(ElementName = "objecttype")]
    public List<Objecttype> Objecttype { get; set; }
  }
}
