
using System;
using System.Net;
using System.IO;
using System.Threading;

namespace Nsm.Downloaders
{
	
	internal delegate void StatusHandler(int id, string action, double progress);
	internal delegate void FinishedHandler(int id);
    
    /// <summary>Abstract base class to download a file</summary>
	internal abstract class AbstractDownloader : IDisposable
	{
        protected const int BUFFER_SIZE = 2048;
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
		/// <param name="action">Short description of the current process</param>
		/// <param name="progress">The progress of the current operation (0.0 to 1.0)</param>
		protected void OnStatus(string action, double progress) {
            if (Status != null) {
                Status(this.Id, action, progress);
            }
		}
		
		/// <summary>Emits the finished signal</summary>
		protected void OnFinished() {
            if (Finished != null) {
                Finished(this.Id);
            }
		}
	}
	
}
