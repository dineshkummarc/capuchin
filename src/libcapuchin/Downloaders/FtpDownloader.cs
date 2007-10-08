
using System;
using System.IO;
using System.Net;

namespace Capuchin.Downloaders
{
	
	
	internal class FtpDownloader : AbstractDownloader
	{
		
		public FtpDownloader(int id, Download dl) 
		 : base(id, dl) { }
		 
		internal override void Download (object startPoint)
		{
			try {
				long startPointLong = Convert.ToInt64(startPoint);
				
				FtpWebRequest webRequest = (FtpWebRequest)WebRequest.Create ( base.dl.Url );
				webRequest.Method = WebRequestMethods.Ftp.DownloadFile;
				webRequest.UseBinary = true;
				webRequest.Credentials = new NetworkCredential("anonymous", "anonymous@gnome.org");
				
				webResponse = webRequest.GetResponse();
				long fileSize = base.webResponse.ContentLength;
				
				Console.WriteLine(fileSize);
				base.strResponse = base.webResponse.GetResponseStream();
				
				// Create a new file stream where we will be saving the data (local drive)
				if (startPointLong == 0)
				{
					base.strLocal = new FileStream(base.dl.LocalFile, FileMode.Create, FileAccess.Write, FileShare.None);
				} else {
					Console.WriteLine(base.strResponse.CanSeek);
					base.strLocal = new FileStream(base.dl.LocalFile, FileMode.Append, FileAccess.Write, FileShare.None);
					base.strResponse.Seek(startPointLong, SeekOrigin.Begin);
				}

				// It will store the current number of bytes we retrieved from the server
				int bytesSize = 0;
				// A buffer for storing and writing the data retrieved from the server
				byte[] downBuffer = new byte[ BUFFER_SIZE ];

				// Loop through the buffer until the buffer is empty
				while ((bytesSize = strResponse.Read(downBuffer, 0, downBuffer.Length)) > 0)
				{
					//
					// Download progress will not be calculated correctly,
					// because ContentLength is always -1 (see Mono bug #81425)
					//
					// Write the data from the buffer to the local hard drive
					strLocal.Write(downBuffer, 0, bytesSize);
					// Invoke the method that updates the form's label and progress bar
					// TODO: Implement speed information for FTP
					base.OnStatus( (double)strLocal.Length/(fileSize + startPointLong), -1 );
				}
			} finally {                
                base.strLocal.Close();
                base.strResponse.Close();
                base.webResponse.Close();
            }
            // Let the world know that we're done            
            base.OnFinished();
		}

	}
}
