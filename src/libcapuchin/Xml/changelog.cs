using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Capuchin.Xml
{
    
   [XmlRootAttribute(ElementName="changelog", Namespace = "", IsNullable = false)]
    public class changelog : Dictionary<string, string>, IXmlSerializable
    {
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        
        public void ReadXml(XmlReader reader)
        {
            reader.Read();
            
            string version = null;
            string text = null;
            while (reader.Name != "changelog")
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        reader.MoveToAttribute("version");
                        version = reader.Value;
                    break;
                    case XmlNodeType.Text:
                        text = reader.Value;
                    break;
                    case XmlNodeType.EndElement:
                        if (version != null && text != null)
                        {
                             this.Add(version, text);
                             version = text = null;
                        }
                    break;
                }
                reader.Read();
            }
            
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach ( string key in this.Keys )
            {
                writer.WriteStartElement("changes");
                writer.WriteAttributeString("version", key);
                writer.WriteEndElement();
            }
        }
    }
    
}
