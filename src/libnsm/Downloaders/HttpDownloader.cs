
using System;
using System.IO;
using System.Net;

namespace Nsm
{
	
	/// <summary>Download files over HTTP</summary>
	internal class HttpDownloader : AbstractDownloader
	{	
		public HttpDownloader(string download_url, string download_dest, string signature, checksum checksumField) 
		 : base(download_url, download_dest, signature, checksumField) { }
	
		internal override void Download(object startPoint)
		{
			// TODO: HTTP rfc says that max 2 connections to a server are allowed
            try {
            	int startPointInt = Convert.ToInt32(startPoint);
            	HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create( base.download_url );
            	webReq.AddRange(startPointInt);
            
            	// Set default authentication for retrieving the file
        		webReq.Credentials = CredentialCache.DefaultCredentials;                
                // Retrieve the response from the server
                webResponse = webReq.GetResponse();
            	// Ask the server for the file size and store it
                long fileSize = webResponse.ContentLength;
            	// Open the URL for download
                strResponse = webResponse.GetResponseStream();
                
                // Create a new file stream where we will be saving the data (local drive)
                if (startPointInt == 0)
                {
                	strLocal = new FileStream(base.local_file, FileMode.Create, FileAccess.Write, FileShare.None);
                } else {
                	strLocal = new FileStream(base.local_file, FileMode.Append, FileAccess.Write, FileShare.None);
                }
                
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
                    base.OnStatus( String.Format("Downloading {0}", Path.GetFileName(this.download_url)), (double)strLocal.Length/fileSize );
                }
                
            } finally {                
                strLocal.Close();
                strResponse.Close();
                webResponse.Close();
            }
		}
	}
}
