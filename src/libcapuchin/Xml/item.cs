using System;
using System.Xml;
using System.Xml.Serialization;
    
namespace Capuchin.Xml
{
    
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class item
    {   
        [XmlElementAttribute("author", typeof(author), IsNullable = false)]
        public author Author;
        
        [XmlElementAttribute("checksum", typeof(checksum), IsNullable = false)]
        public checksum Checksum;
        
        [XmlElementAttribute("description", typeof(string), IsNullable = false)]
        public string Description;
        
        [XmlElementAttribute("id", typeof(string))]
        public string Id;
        
        [XmlElementAttribute("name", typeof(string))]
        public string Name;
        
        [XmlElementAttribute("signature", typeof(string), DataType = "anyURI", IsNullable = false)]
        public string Signature;
        
        [XmlArrayAttribute("tags", IsNullable=false)]
        [XmlArrayItemAttribute("tag", typeof(string), IsNullable=false)]
        public string[] Tags;
        
        [XmlElementAttribute("url", typeof(string), DataType = "anyURI")]
        public string Url;
        
        [XmlElementAttribute("version", typeof(string))]
        public string Version;
        
        [XmlElementAttribute("changelog", typeof(changelog), IsNullable = false)]
        public changelog Changelog;        
    }
    
}
