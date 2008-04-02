
using System;
using System.Net;
using System.IO;
using System.Threading;

namespace Capuchin.Downloaders
{
    
    internal delegate void StatusHandler(int id, double progress, int speed);
    internal delegate void FinishedHandler(AbstractDownloader downloader, int id);
    
    /// <summary>Abstract base class to download a file</summary>
    internal abstract class AbstractDownloader : IDisposable
    {
        protected const int BUFFER_SIZE = 4096;
        
        private bool disposed = false;        
        
        protected Download dl;
        
        protected Stream strResponse;
        protected Stream strLocal;
        protected WebResponse webResponse;
        
        public event StatusHandler Status;
        public event FinishedHandler Finished;
        public readonly int Id;
        
        /// <param name="id">Unique ID for the download</param>
        /// <param name="dl"><see cref="Download"> instance that contains
        /// information about the download</param>
        public AbstractDownloader(int id, Download dl)
        {
            this.Id = id;
            this.dl = dl;
        }
        
        ~AbstractDownloader()
        {
            this.Dispose();
        }
        
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.dl = null;
                this.strResponse = null;
                this.strLocal = null;
                this.webResponse = null;
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        
        /// <summary>Download file</summary>  
        internal void Download()
        {
            this.Download(0);
        }
        
        /// <summary>Download file</summary>
        /// <param name="startPoint">Start downloading at the given point</start>
        internal abstract void Download(object startPoint);
        
        /// <summary>Emits status signal</summary>
        /// <param name="progress">The progress of the current operation (0.0 to 1.0)</param>
        protected void OnStatus(double progress, int speed) {
            if (Status != null) {
                Status(this.Id, progress, speed);
            }
        }
        
        /// <summary>Emits the finished signal</summary>
        protected void OnFinished() {
            if (Finished != null) {
                Finished(this, this.Id);
            }
        }
    }
    
}
