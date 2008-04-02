
using System;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace Capuchin.Downloaders
{
    
    /// <summary>Download files over HTTP</summary>
    internal class HttpDownloader : AbstractDownloader
    {    
        public HttpDownloader(int id, Download dl) 
         : base(id, dl) { }
    
        internal override void Download(object startPoint)
        {
            // Measure how long the download took
            Stopwatch timer = new Stopwatch();
                
            // TODO: HTTP rfc says that max 2 connections to a server are allowed
            try {
                int startPointInt = Convert.ToInt32(startPoint);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create( base.dl.Url );
                webReq.AddRange(startPointInt);
            
                // Set default authentication for retrieving the file
                webReq.Credentials = CredentialCache.DefaultCredentials;                
                // Retrieve the response from the server
                webResponse = webReq.GetResponse();
                // Ask the server for the file size and store it
                long fileSize = webResponse.ContentLength;
                // Open the URL for download
                strResponse = webResponse.GetResponseStream();
				
				string disposition = webResponse.Headers.Get ("Content-Disposition");
				string localFile;
				if (disposition != null && disposition.ToLower().StartsWith ("attachment"))
				{
					// Looks like: Content-Disposition: attachment; filename=genome.jpeg;
					string filename = disposition.Split (';')[1].Split('=')[1].Replace("\"", "");
					localFile = Path.Combine( base.dl.Destination, filename);
				} else {
					localFile = Path.Combine( base.dl.Destination, Path.GetFileName(base.dl.Url) );
				}
                
                // Create a new file stream where we will be saving the data (local drive)
                if (startPointInt == 0)
                {
                    strLocal = new FileStream(localFile, FileMode.Create, FileAccess.Write, FileShare.None);
                } else {
                    strLocal = new FileStream(localFile, FileMode.Append, FileAccess.Write, FileShare.None);
                }
                
                // It will store the current number of bytes we retrieved from the server
                int bytesSize = 0;
                // A buffer for storing and writing the data retrieved from the server
                byte[] downBuffer = new byte[ BUFFER_SIZE ];

                // Loop through the buffer until the buffer is empty
                timer.Start();
                while ( (bytesSize = strResponse.Read(downBuffer, 0, downBuffer.Length)) > 0 )
                {
                    // Write the data from the buffer to the local hard drive
                    strLocal.Write(downBuffer, 0, bytesSize);
                    
                    // Compute download progress
                    double progress = (double)strLocal.Length/(fileSize + startPointInt);
                    // Compute download speed
                    int speed = (int)( strLocal.Length / (timer.ElapsedMilliseconds / 1000.0) );
                    // Invoke the method that updates the form's label and progress bar
                    base.OnStatus( progress, speed );
                }
                
            } finally {
                timer.Stop();
                strLocal.Close();
                strResponse.Close();
                webResponse.Close();
            }
            // Let the world know that we're done            
            base.OnFinished();
        }
    }
}
