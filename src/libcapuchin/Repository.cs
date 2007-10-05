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

        private author authorField;
        private checksum checksumField;
        private string descriptionField;
        private string idField;
        private string nameField;
        private string signatureField;
        private string[] tagsField;
        private string urlField;
        private string versionField;
        
        [System.Xml.Serialization.XmlElementAttribute("author", typeof(author), IsNullable = false)]
        public author Author
        {
            get
            {
                return this.authorField;
            }
            set
            {
                this.authorField = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("checksum", typeof(checksum), IsNullable = false)]
        public checksum Checksum
        {
            get
            {
                return this.checksumField;
            }
            set
            {
                this.checksumField = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("description", typeof(string), IsNullable = false)]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("id", typeof(string))]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("name", typeof(string))]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("signature", typeof(string), DataType = "anyURI", IsNullable = false)]
        public string Signature
        {
            get
            {
                return this.signatureField;
            }
            set
            {
                this.signatureField = value;
            }
        }
        
        [XmlArrayAttribute("tags", IsNullable=false)]
        [XmlArrayItemAttribute("tag", typeof(string), IsNullable=false)]
        public string[] Tags
        {
            get
            {
                return this.tagsField;
            }
            set
            {
                this.tagsField = value;
            }
        }
        
        [System.Xml.Serialization.XmlElementAttribute("url", typeof(string), DataType = "anyURI")]
        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("version", typeof(string))]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
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
