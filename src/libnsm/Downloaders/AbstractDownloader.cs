
using System;
using System.Net;
using System.IO;
using System.Threading;

namespace Nsm.Downloaders
{
	
	internal delegate void StatusHandler(int id, string action, double progress);
    
    /// <summary>Abstract base class to download a file</summary>
	internal abstract class AbstractDownloader : IDisposable
	{
        protected const int BUFFER_SIZE = 2048;
	    private bool disposed = false;	    
	    
		protected string download_url;
	    protected string download_dest;
	    protected string local_file;
	    protected string signature;
	    protected checksum checksumField;
		
		protected Stream strResponse;
        protected Stream strLocal;
        protected WebResponse webResponse;
		
		public event StatusHandler Status;
		public readonly int Id;
	    
	    /// <param name="download_url">Url of file to download</param>
	    /// <param name="download_dest">Directory where to save the file</param>
	    /// <param name="checksumField">Checksum instance of file or null</param>
		public AbstractDownloader(int id, string download_url, string download_dest, string signature, checksum checksumField)
		{
			this.Id = id;
		    this.checksumField = checksumField;
		    this.download_url = download_url;
		    this.download_dest = download_dest;
		    this.signature = signature;
		    this.local_file = Path.Combine(this.download_dest, Path.GetFileName(this.download_url));
		}
		
		~AbstractDownloader()
		{
		    this.Dispose();
		}
		
		public void Dispose()
		{
		    if (!this.disposed)
		    {
		        this.checksumField = null;
		        this.strResponse = null;
		        this.strLocal = null;
		        this.webResponse = null;
		        this.disposed = true;
		        GC.SuppressFinalize(this);
		    }
		}
		
		/// <summary>Starts downloading and verfification</summary>
		internal void Run()
		{
            this.Download();
            
            if (this.checksumField != null)
            {
                FileStream fs = new FileStream( this.local_file, FileMode.Open );
                Verification.ChecksumVerifier cv = new Verification.ChecksumVerifier( fs, this.checksumField );
                fs.Close();
                
                Console.WriteLine("*** Checksum valid: {0}", cv.IsValid);
                if (!cv.IsValid)
                {
                    // Checksum is invalid
                    File.Delete(this.local_file);
                    return;
                }
            }
            
            if (this.signature != null)
            {
                Verification.GnuPGVerifier gv = new Verification.GnuPGVerifier(this.local_file, this.signature);
                
                Console.WriteLine("*** Signature valid: {0}", gv.IsValid);
                if (!gv.IsValid)
                {
                    // Signature invalid
                    File.Delete(this.local_file);
                    return;
                }
            }
            
            this.OnStatus("Extracting", -1.0);
            Compression.Decompresser decomp = new Compression.Decompresser(this.local_file, download_dest);
            Thread decompthread = new Thread( new ThreadStart(decomp.Run) );
            decompthread.Start();
            decompthread.Join();
		 
            if (decomp.DeleteFile)
                File.Delete(this.local_file);
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
	}
	
}
