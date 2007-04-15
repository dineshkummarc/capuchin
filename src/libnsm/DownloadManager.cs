
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NDesk.DBus;
using org.freedesktop.DBus;
using Nsm.Downloaders;

namespace Nsm
{
	/// <summary>Class that manages downloads</summary>
	[Interface("org.gnome.NewStuffManager.DownloadManager")]
	public class DownloadManager : MarshalByRefObject
	{
	    protected Dictionary<int, Download> Downloads;
	    private int downloadsIndex;
	    
	    public delegate void DownloaderDel(object startPoint);
	    public delegate void DownloadStatusHandler(int id, string action, double progress);
	    public event DownloadStatusHandler DownloadStatus;
	    
	    public DownloadManager()
	    {
			this.Downloads = new Dictionary<int, Download>();
			this.downloadsIndex = 0;
	   	}
	    
		/// <summary>Download a file using the DownloadManager</summary>
    	public virtual int DownloadFile(string download_url, string download_dest)
    	{
			return this.DownloadFile(download_url, download_dest, null, null);
    	}
    	
    	/// <param name="download_url">Url of file to download</param>
	    /// <param name="download_dest">Directory where to save the file</param>
	    /// <param name="checksumField">Checksum instance of file or null</param>
	    /// <returns>
	    /// An <see cref="Nsm.Downloaders.AbstractDownloader" /> instance to
	    /// download the given file
	    /// </returns>
		internal int DownloadFile (string download_url, string download_dest, string signature, checksum checksumField)
		{
			AbstractDownloader dl = this.GetDownloader(this.downloadsIndex, download_url, download_dest,signature, checksumField);
            dl.Status += new StatusHandler( this.OnDownloadStatus );
            
            Thread dlThread = new Thread( new ThreadStart(dl.Download) );
			this.Downloads.Add( this.downloadsIndex, new Download(download_url, download_dest, dlThread) );
			this.downloadsIndex++;
			dlThread.Start();
			
			Console.WriteLine("*** Started downloading file with id '{0}'", this.downloadsIndex-1);
			
			return (this.downloadsIndex-1);
		}
    	
    	/// <summary>Pause download</summary>
    	/// <param name="id">Download id as returned by <see cref="Nsm.DownloadManager.DownloadFile" /></param>
    	public virtual void PauseDownload(int id)
    	{
    		Console.WriteLine("*** Paused download with id '{0}'", id);
    		// Kill Downloader Thread
    		this.Downloads[id].Downloader.Abort();
    	}
    	
    	/// <summary>Abort download</summary>
    	/// <param name="id">Download id as returned by <see cref="Nsm.DownloadManager.DownloadFile" /></param>
    	public virtual void AbortDownload(int id)
    	{
    		Console.WriteLine("*** Aborted download with id '{0}'", id);
    	
    		this.PauseDownload(id);
    		File.Delete(this.Downloads[id].LocalFile);
    		this.Downloads.Remove(id);
    	}
    	
    	/// <summary>Resume download</summary>
    	/// <param name="id">Download id as returned by <see cref="Nsm.DownloadManager.DownloadFile" /></param>
    	public virtual void ResumeDownload(int id)
    	{
    		// Get file info
    		FileInfo f = new FileInfo( this.Downloads[id].LocalFile );
    		// Get Downloader
			AbstractDownloader dl = this.GetDownloader(id, this.Downloads[id].Url, this.Downloads[id].Destination, null, null);
            dl.Status += new StatusHandler( this.OnDownloadStatus );
            // Start Thread
            Thread dlThread = new Thread( new ParameterizedThreadStart(dl.Download) );			
			this.Downloads[id] = new Download(this.Downloads[id].Url, this.Downloads[id].Destination, dlThread);
			dlThread.Start(f.Length);
			
			Console.WriteLine("*** Resuming download with id '{0}'", id);
    	}
    	
    	protected void OnDownloadStatus(int id, string action, double progress)
    	{
    		if (DownloadStatus != null)
    		{
    			DownloadStatus(id, action, progress);
    		}
    		if ((int)progress == 1)
    		{
    			Console.WriteLine("*** Finished downloading file with id '{0}'", id);
    			// Remove Download
    			this.Downloads.Remove(id);
    		}
    	}
    	
    	/// <summary>Returns the appropriate <see cref="Nsm.Downloaders.AbstractDownloader" /></summary>
    	internal AbstractDownloader GetDownloader(int id, string download_url, string download_dest, string signature, checksum checksumField)
    	{
    		Uri uri = new Uri(download_url);
            
            if (uri.Scheme == "http")
            {
            	return new HttpDownloader(id, download_url, download_dest, signature, checksumField);
            } else if (uri.Scheme == "ftp") {
            	return new FtpDownloader(id, download_url, download_dest, signature, checksumField);
            } else {
            	throw new NotImplementedException("Scheme '"+ uri.Scheme + "' is not supported");            	
            }
        }
    	
	}
}
