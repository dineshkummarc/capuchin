using System;
using System.Xml;
using System.Xml.Serialization;

namespace Capuchin.Xml
{
    
    [XmlRootAttribute(ElementName="author", Namespace = "", IsNullable = false)]
    public class author
    {
        [XmlAttributeAttribute("name", typeof(string))]
        public string Name;
                               
        [XmlAttributeAttribute("email", typeof(string))]
        public string Email;
    }
    
}