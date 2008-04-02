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
using System.Threading;
using Capuchin.Xml;

namespace Capuchin
{
    
    /// <summary>A download as used by <see cref="Nsm.NewStuffManager" /></summary>    
    internal class Download
    {
        public readonly int Id;
        public readonly string Url;
        public readonly string Destination;
        public readonly string LocalFile;        
        public readonly string Signature;
        public readonly checksum Checksum;
        public Thread Downloader;
        
        /// <param name="url">URL of file to download</param>
        /// <param name="dest">Directory where to save the file</param>
        /// <param name="dlThread">
        ///     Thread that downloads the file.
        ///        In the Thread runs a <see cref="Nsm.Downloaders.AbstractDownloader" /> instance.
        /// </param>
        public Download(int id, string url, string dest, string sig, checksum checksumField, Thread dlThread)
        {
            this.Id = id;
            this.Url = url;
            this.Destination = dest;
            this.Signature = sig;
            this.Checksum = checksumField;
            this.LocalFile = Path.Combine( this.Destination, Path.GetFileName(this.Url) );
            this.Downloader = dlThread;
        }
        
        public Download(int id, string url, string dest, string sig, checksum checksumField) : this(id, url, dest, sig, checksumField, null)
        {
        
        }
            
    }
}
