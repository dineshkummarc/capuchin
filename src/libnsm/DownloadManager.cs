
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NDesk.DBus;
using org.freedesktop.DBus;
using Nsm.Downloaders;

namespace Nsm
{
	public delegate void DownloadManagerStatusHandler(int id, double progress);
	public delegate void DownloadManagerFinishedHandler(int id);
	
	[Interface("org.gnome.NewStuffManager.DownloadManager")]
	public interface IDownloadManager
	{
	    event DownloadManagerStatusHandler DownloadStatus;
		event DownloadManagerFinishedHandler DownloadFinished;
		int DownloadFile(string download_url, string download_dest);
		void PauseDownload(int id);
		void AbortDownload(int id);
		void ResumeDownload(int id);
	}
	
	/// <summary>Class that manages downloads</summary>
	public class DownloadManager : IDownloadManager
	{
		public event DownloadManagerStatusHandler DownloadStatus;
		public event DownloadManagerFinishedHandler DownloadFinished;
		
		public delegate void DownloaderDel(object startPoint);
		
	    protected Dictionary<int, Download> Downloads;
	    
	    private int downloadsIndex;
	    
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
			Download dl = new Download(this.downloadsIndex, download_url, download_dest, signature, checksumField);
			
			AbstractDownloader downloader = this.GetDownloader(this.downloadsIndex, dl);
            downloader.Status += new StatusHandler( this.OnDownloadStatus );
            downloader.Finished += new FinishedHandler( this.DownloadFinishedCallback );
            
            Thread downloaderThread = new Thread( new ThreadStart(downloader.Download) );
            dl.Downloader = downloaderThread;
			this.Downloads.Add( this.downloadsIndex, dl );
			this.downloadsIndex++;
			downloaderThread.Start();
			
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
			AbstractDownloader downloader = this.GetDownloader(id, this.Downloads[id]);
            downloader.Status += new StatusHandler( this.OnDownloadStatus );
            downloader.Finished += new FinishedHandler( this.DownloadFinishedCallback );
            // Start Thread
            Thread downloaderThread = new Thread( new ParameterizedThreadStart(downloader.Download) );
            this.Downloads[id] = new Download ( id,
            	this.Downloads[id].Url,
            	this.Downloads[id].Destination,
            	this.Downloads[id].Signature,
            	this.Downloads[id].Checksum,
            	downloaderThread);
			downloaderThread.Start(f.Length);
			
			Console.WriteLine("*** Resuming download with id '{0}'", id);
    	}
    	
    	protected void OnDownloadStatus(int id, double progress)
    	{
    		if (DownloadStatus != null)
    		{
    			DownloadStatus(id, progress);
    		}
    	}
    	
    	/// <summary>Emits the finished signal</summary>
		protected void OnDownloadFinished(int id) {
            if (DownloadFinished != null) {
                DownloadFinished(id);
            }
		}
    		
    	protected void DownloadFinishedCallback(int id)
    	{
			Console.WriteLine("*** Finished downloading file with id '{0}'", id);
            
			// Remove Download
			this.Downloads.Remove(id);
			
			this.OnDownloadFinished(id);
    	}
    	
    	/// <summary>Returns the appropriate <see cref="Nsm.Downloaders.AbstractDownloader" /></summary>
    	internal AbstractDownloader GetDownloader(int id, Download dl)
    	{
    		Uri uri = new Uri(dl.Url);
            
            if (uri.Scheme == "http")
            {
            	return new HttpDownloader(id, dl);
            } else if (uri.Scheme == "ftp") {
            	return new FtpDownloader(id, dl);
            } else {
            	throw new NotImplementedException("Scheme '"+ uri.Scheme + "' is not supported");            	
            }
        }
    	
	}
}
