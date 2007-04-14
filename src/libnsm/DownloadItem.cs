
using System;
using System.Threading;
using Nsm.Downloaders;

namespace Nsm
{    
    /// <summary>Downloads a file</summary>
	internal class DownloadItem
	{
		protected string download_url;
	    protected string download_dest;
	    protected string signature;
	    protected checksum checksumField;
	    public readonly AbstractDownloader Downloader;
	   
	    /// <param name="download_url">Url of file to download</param>
	    /// <param name="download_dest">Directory where to save the file</param>
	    /// <param name="checksumField">Checksum instance of file or null</param>
		public DownloadItem(string download_url, string download_dest, string signature, checksum checksumField)
		{
		    this.checksumField = checksumField;
		    this.download_url = download_url;
		    this.download_dest = download_dest;
		    this.signature = signature;
		          
            Uri uri = new Uri(this.download_url);
            
            if (uri.Scheme == "http")
            {
            	this.Downloader = new HttpDownloader(download_url, download_dest, signature, checksumField);
            } else if (uri.Scheme == "ftp") {
            	this.Downloader = new FtpDownloader(download_url, download_dest, signature, checksumField);
            } else {
            	throw new NotImplementedException("Scheme "+ uri.Scheme + " is not supported");            	
            }
		}
	}
	
}
