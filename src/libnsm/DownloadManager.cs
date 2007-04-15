
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NDesk.DBus;
using org.freedesktop.DBus;
using Nsm.Downloaders;

namespace Nsm
{
	
	[Interface("org.gnome.NewStuffManager.DownloadManager")]
	public class DownloadManager : MarshalByRefObject
	{
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
    		AbstractDownloader dl = DownloadClient.GetDownloader(download_url, download_dest, null, null);
			
			dl.Status += new StatusHandler(
			 delegate(string action, double progress) { Console.WriteLine("{0}: {1}", action, progress); }
			);
			
			Thread dlThread = new Thread( new ThreadStart(dl.Download) );
			this.Downloads.Add( this.downloadsIndex, new Download(download_url, download_dest, dlThread) );
			this.downloadsIndex++;
			dlThread.Start();
			
			return (this.downloadsIndex-1);
    	}
    	
    	public virtual void PauseDownload(int id)
    	{
    		this.Downloads[id].Downloader.Abort();
    	}
    	
    	public virtual void AbortDownload(int id)
    	{
    		this.PauseDownload(id);
    		File.Delete(this.Downloads[id].LocalFile);
    		this.Downloads.Remove(id);
    	}
    	
    	public virtual void ResumeDownload(int id)
    	{
    		FileInfo f = new FileInfo( this.Downloads[id].LocalFile );
    		
    		AbstractDownloader dl = DownloadClient.GetDownloader(this.Downloads[id].Url, this.Downloads[id].Destination, null, null);
			Console.WriteLine("Resuming at {0}", f.Length);
			dl.Status += new Nsm.Downloaders.StatusHandler(
			 delegate(string action, double progress) { Console.WriteLine("***Resume*** {0}: {1}", action, progress); }
			);
			
			Thread dlThread = new Thread( new ParameterizedThreadStart(dl.Download) );
			this.Downloads[id] = new Download(this.Downloads[id].Url, this.Downloads[id].Destination, dlThread);
			dlThread.Start(f.Length);
    	}
    	
	}
}
