
using System;
using System.Net;
using System.IO;
using System.Threading;

namespace Nsm
{
	
	internal delegate void StatusHandler(string action, double progress);
    
    /// <summary>Downloads a file</summary>
	internal class Downloader : IDisposable
	{
        protected const int BUFFER_SIZE = 2048;
	    private bool disposed = false;
	    
		protected string download_url;
	    protected string download_dest;
	    protected string signature;
	    protected checksum checksumField;
		
		private Stream strResponse;
        private Stream strLocal;
        private HttpWebResponse webResponse;
		
		public event StatusHandler Status;
	    
	    /// <param name="download_url">Url of file to download</param>
	    /// <param name="download_dest">Directory where to save the file</param>
	    /// <param name="checksumField">Checksum instance of file or null</param>
		public Downloader(string download_url, string download_dest, string signature, checksum checksumField)
		{
		    this.checksumField = checksumField;
		    this.download_url = download_url;
		    this.download_dest = download_dest;
		    this.signature = signature;
		}
		
		~Downloader()
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
		public void Run()
		{
            string filename = Path.GetFileName(this.download_url);
            string localFile = Path.Combine(this.download_dest, filename);
            // TODO: HTTP rfc says that max 2 connections to a server are allowed
            try {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create( this.download_url );
                webResponse = (HttpWebResponse)webReq.GetResponse();
            
                long fileSize = webResponse.ContentLength;
            
                strResponse = webResponse.GetResponseStream();
                strLocal = new FileStream(localFile, FileMode.Create, FileAccess.Write, FileShare.None);
                
                // It will store the current number of bytes we retrieved from the server
                int bytesSize = 0;
                // A buffer for storing and writing the data retrieved from the server
                byte[] downBuffer = new byte[ BUFFER_SIZE ];

                // Loop through the buffer until the buffer is empty
                while ((bytesSize = strResponse.Read(downBuffer, 0, downBuffer.Length)) > 0)
                {
                    // Write the data from the buffer to the local hard drive
                    strLocal.Write(downBuffer, 0, bytesSize);
                    // Invoke the method that updates the form's label and progress bar
                    this.OnStatus( String.Format("Downloading {0}", filename), (double)strLocal.Length/fileSize );
                }
                
            } finally {                
                strLocal.Close();
                strResponse.Close();
                webResponse.Close();
            }
            /*
            using(WebClient wc = new WebClient()) {
                try {
                    // Create a request to the file we are downloading
                     HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(this.download_url);
                     // Set default authentication for retrieving the file
                     //webRequest.Credentials = CredentialCache.DefaultCredentials;
                     // Retrieve the response from the server
                     webResponse = (HttpWebResponse)webRequest.GetResponse();
                     // Ask the server for the file size and store it
                     long fileSize = webResponse.ContentLength;
                     
                     // Open the URL for download
                     Console.WriteLine("BREAKPOINT");
                    strResponse = wc.OpenRead(this.download_url);
                    Console.WriteLine("AFTER BREAKPOINT");
                     // Create a new file stream where we will be saving the data (local drive)
                     strLocal = new FileStream(localFile, FileMode.Create, FileAccess.Write, FileShare.None);

                     // It will store the current number of bytes we retrieved from the server
                     int bytesSize = 0;
                     // A buffer for storing and writing the data retrieved from the server
                     byte[] downBuffer = new byte[ BUFFER_SIZE ];

                     // Loop through the buffer until the buffer is empty
                     while ((bytesSize = strResponse.Read(downBuffer, 0, downBuffer.Length)) > 0)
                     {
                         // Write the data from the buffer to the local hard drive
                         strLocal.Write(downBuffer, 0, bytesSize);
                         // Invoke the method that updates the form's label and progress bar
                         this.OnStatus( String.Format("Downloading {0}", filename), (double)strLocal.Length/fileSize );
                     }
                } finally {
                    // When the above code has ended, close the streams
                    strResponse.Close();
                    strLocal.Close();
                    webResponse.Close();
                }
            }
            */
            if (this.checksumField != null)
            {
                FileStream fs = new FileStream( localFile, FileMode.Open );
                Verification.ChecksumVerifier cv = new Verification.ChecksumVerifier( fs, this.checksumField );
                fs.Close();
                
                Console.WriteLine("*** Checksum valid: {0}", cv.IsValid);
                if (!cv.IsValid)
                {
                    // Checksum is invalid
                    File.Delete(localFile);
                    return;
                }
            }
            
            if (this.signature != null)
            {
                Verification.GnuPGVerifier gv = new Verification.GnuPGVerifier(localFile, this.signature);
                
                Console.WriteLine("*** Signature valid: {0}", gv.IsValid);
                if (!gv.IsValid)
                {
                    // Signature invalid
                    File.Delete(localFile);
                    return;
                }
            }
            
            this.OnStatus("Extracting", -1.0);
            Compression.Decompresser decomp = new Compression.Decompresser(localFile, download_dest);
            Thread decompthread = new Thread( new ThreadStart(decomp.Run) );
            decompthread.Start();
            decompthread.Join();
		 
            if (decomp.DeleteFile)
                File.Delete(localFile);
		}
		
		/// <summary>Emits status signal</summary>
		/// <param name="action">Short description of the current process</param>
		/// <param name="progress">The progress of the current operation (0.0 to 1.0)</param>
		private void OnStatus(string action, double progress) {
            if (Status != null) {
                Status(action, progress);
            }
		}
	}
	
}
