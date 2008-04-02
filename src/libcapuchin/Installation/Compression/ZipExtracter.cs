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
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Capuchin.Installation.Compression
{
    
    internal class ZipExtracter : IExtracter
    {
        
        public void Extract(string path, string dest_dir)
        {
          ZipInputStream zipstream = new ZipInputStream(new FileStream( path, FileMode.Open));
          
          ZipEntry theEntry;
          while ((theEntry = zipstream.GetNextEntry()) != null) {
            string directoryName = Path.GetDirectoryName(theEntry.Name);
            string fileName      = Path.GetFileName(theEntry.Name);
            
            // create directory
            if (directoryName != String.Empty)
                Directory.CreateDirectory(directoryName);
            
            if (fileName != String.Empty) {
                FileStream streamWriter = File.Create( Path.Combine( dest_dir, theEntry.Name) );
                
                int size = 0;
                byte[] buffer = new byte[2048];
                while ((size = zipstream.Read(buffer, 0, buffer.Length)) > 0) {                    
                    streamWriter.Write(buffer, 0, size);
                }
                streamWriter.Close();
            }
          }
          zipstream.Close();
        }
    }
}
