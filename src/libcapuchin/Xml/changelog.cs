/*
 * This file is part of Capuchin
 * 
 * Copyright (C) Sebastian Pölsterl 2008 <marduk@k-d-w.org>
 * 
 * Capuchin is free software.
 * 
 * You may redistribute it and/or modify it under the terms of the
 * GNU General Public License, as published by the Free Software
 * Foundation; either version 2 of the License, or (at your option)
 * any later version.
 * 
 * Capuchin is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Capuchin.  If not, write to:
 * 	The Free Software Foundation, Inc.,
 * 	51 Franklin Street, Fifth Floor
 * 	Boston, MA  02110-1301, USA.
 */

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
