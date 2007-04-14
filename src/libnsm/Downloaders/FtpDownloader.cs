
using System;

namespace Nsm.Downloaders
{
	
	
	internal class FtpDownloader : AbstractDownloader
	{
		
		public FtpDownloader(string download_url, string download_dest, string signature, checksum checksumField) 
		 : base(download_url, download_dest, signature, checksumField) { }
		 
		internal override void Download (object startPoint)
		{
			
		}

	}
}
