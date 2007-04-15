
using System;
using System.IO;
using System.Threading;

namespace Nsm
{
	
	/// <summary>A download as used by <see cref="Nsm.NewStuffManager" /></summary>	
	internal struct Download
	{
		public string Url;
		public string Destination;
		public string LocalFile;
		public Thread Downloader;
		
		/// <param name="url">URL of file to download</param>
		/// <param name="dest">Directory where to save the file</param>
		/// <param name="dlThread">
		/// 	Thread that downloads the file.
		///		In the Thread runs a <see cref="Nsm.Downloaders.AbstractDownloader" /> instance.
		/// </param>
		public Download(string url, string dest, Thread dlThread)
		{
			this.Url = url;
			this.Destination = dest;
			this.Downloader = dlThread;
			this.LocalFile = Path.Combine( this.Destination, Path.GetFileName(this.Url) );
		}
			
	}
}
