using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Capuchin.Xml
{
    
    [XmlRootAttribute(ElementName="author", Namespace = "", IsNullable = false)]
    public class author : Dictionary<string, string>, IXmlSerializable
    {
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        
        public void ReadXml(XmlReader reader)
        {   
            reader.MoveToAttribute("name");
            this.Add("name", reader.Value);
            
            reader.MoveToAttribute("email");
            this.Add("email", reader.Value);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", this["name"]);
            writer.WriteAttributeString("email", this["email"]);
        }
    }
    
}
