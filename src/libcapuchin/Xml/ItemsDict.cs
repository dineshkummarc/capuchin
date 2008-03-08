using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Capuchin.Xml
{
    
    internal class ItemsDict : Dictionary<string, item>, IXmlSerializable
    {
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        
        public void ReadXml(XmlReader reader)
        {   
            XmlSerializer itemSer = new XmlSerializer(typeof(item));
            
            reader.Read();
            
            while (reader.NodeType != XmlNodeType.EndElement)
            {   
                item dictItem = (item)itemSer.Deserialize(reader);
                
                this.Add(dictItem.Id, dictItem);
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer itemSer = new XmlSerializer(typeof(item));
            
            foreach (string key in this.Keys)
            {                
                itemSer.Serialize(writer, this[key]);
            }
        }
    }
    
}
