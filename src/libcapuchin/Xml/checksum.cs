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
using System.Xml;
using System.Xml.Serialization;

namespace Capuchin.Xml
{
    
    public enum checksumType
    {
        sha1,
        md5,
    }
    
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class checksum
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
