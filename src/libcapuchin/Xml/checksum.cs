using System;
using System.Xml;
using System.Xml.Serialization;

namespace Capuchin.Xml
{
    
    internal enum checksumType
    {
        sha1,
        md5,
    }
    
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    internal class checksum
    {

        private checksumType typeField;
        private string textField;

        [XmlAttributeAttribute()]
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

        [XmlTextAttribute()]
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
    
}
