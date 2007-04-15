
using System;
using System.Threading;
using Nsm.Downloaders;

namespace Nsm
{    
    /// <summary>
    /// Provides the correct <see cref="Nsm.Downloaders.AbstractDownloader" />
    /// for downloading
    /// </summary>
	internal class DownloadClient
	{ 
	    /// <param name="download_url">Url of file to download</param>
	    /// <param name="download_dest">Directory where to save the file</param>
	    /// <param name="checksumField">Checksum instance of file or null</param>
	    /// <returns>
	    /// An <see cref="Nsm.Downloaders.AbstractDownloader" /> instance to
	    /// download the given file
	    /// </returns>
		public static AbstractDownloader GetDownloader(string download_url, string download_dest, string signature, checksum checksumField)
		{
            Uri uri = new Uri(download_url);
            
            if (uri.Scheme == "http")
            {
            	return new HttpDownloader(download_url, download_dest, signature, checksumField);
            } else if (uri.Scheme == "ftp") {
            	return new FtpDownloader(download_url, download_dest, signature, checksumField);
            } else {
            	throw new NotImplementedException("Scheme "+ uri.Scheme + " is not supported");            	
            }
		}
	}
	
}
