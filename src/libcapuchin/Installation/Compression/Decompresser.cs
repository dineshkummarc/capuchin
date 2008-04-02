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
 *     The Free Software Foundation, Inc.,
 *     51 Franklin Street, Fifth Floor
 *     Boston, MA  02110-1301, USA.
 */

using System;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using Capuchin.Logging;

namespace Capuchin.Installation.Compression
{
    internal class Decompresser
    {
        private string path;
        private string dest_dir;
        private bool deleteField;
        
        public bool DeleteFile
        {
            get { return this.deleteField; }
        }
        
        public Decompresser(string path, string dest_dir)
        {
            this.path = path;
            this.dest_dir = dest_dir;
            this.deleteField = false;
        }
        
        public void Run() {
            Log.Info("Decompressing {0} to {1}", this.path, this.dest_dir);
            
            string mime_type = Gnome.Vfs.MimeType.GetMimeTypeForUri(path);

            IExtracter extracter = null;
            if (mime_type == "application/x-bzip-compressed-tar"){
                extracter = new TarBz2Extracter();
                this.deleteField = true;
            } else if (mime_type == "application/x-compressed-tar") {
                extracter = new TarGzExtracter();
                this.deleteField = true;
            } else if (mime_type == "application/zip") {
                extracter = new ZipExtracter();
                this.deleteField = true;
            }
            // Do nothing for "text/x-python"
            if (extracter != null)
                extracter.Extract(this.path, this.dest_dir);
            }
    }
    
}
