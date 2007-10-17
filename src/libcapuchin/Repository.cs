using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Capuchin
{
    [XmlRootAttribute(ElementName="repository", Namespace="", IsNullable=false)]
    public class Repository
    {
        public string application;
        
        [XmlElementAttribute("installpath")]
        public string installpath;
        
        [XmlElementAttribute("items")]
        public ItemsDict items;
    }
    
    public class ItemsDict : Dictionary<string, item>, IXmlSerializable
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

    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class item
    {   
        [System.Xml.Serialization.XmlElementAttribute("author", typeof(author), IsNullable = false)]
        public author Author;
        
        [System.Xml.Serialization.XmlElementAttribute("checksum", typeof(checksum), IsNullable = false)]
        public checksum Checksum;
        
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(string), IsNullable = false)]
        public string Description;
        
        [System.Xml.Serialization.XmlElementAttribute("id", typeof(string))]
        public string Id;
        
        [System.Xml.Serialization.XmlElementAttribute("name", typeof(string))]
        public string Name;
        
        [System.Xml.Serialization.XmlElementAttribute("signature", typeof(string), DataType = "anyURI", IsNullable = false)]
        public string Signature;
        
        [XmlArrayAttribute("tags", IsNullable=false)]
        [XmlArrayItemAttribute("tag", typeof(string), IsNullable=false)]
        public string[] Tags;
        
        [System.Xml.Serialization.XmlElementAttribute("url", typeof(string), DataType = "anyURI")]
        public string Url;
        
        [System.Xml.Serialization.XmlElementAttribute("version", typeof(string))]
        public string Version;
        
        [System.Xml.Serialization.XmlElementAttribute("changelog", typeof(changelog), IsNullable = false)]
        public changelog Changelog;        
    }
    
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
    
    
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class checksum
    {

        private checksumType typeField;
        private string textField;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public checksumType type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        [System.Xml.Serialization.XmlTextAttribute()]
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    public enum checksumType
    {

        sha1,
        md5,
    }
   
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
