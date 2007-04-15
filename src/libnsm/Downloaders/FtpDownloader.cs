
using System;

namespace Nsm.Downloaders
{
	
	
	internal class FtpDownloader : AbstractDownloader
	{
		
		public FtpDownloader(int id, string download_url, string download_dest, string signature, checksum checksumField) 
		 : base(id, download_url, download_dest, signature, checksumField) { }
		 
		internal override void Download (object startPoint)
		{
			
		}

	}
}
