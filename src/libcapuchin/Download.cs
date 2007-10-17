
using System;
using System.IO;
using System.Threading;

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
