using System;
using System.Xml;
using System.Xml.Serialization;

namespace Capuchin.Xml
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
}
