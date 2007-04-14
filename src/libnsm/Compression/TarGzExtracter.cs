
using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;

namespace Nsm.Compression
{
	
	
	internal class TarGzExtracter : IExtracter
	{
		
		public void Extract(string path, string dest_dir)
		{
            GZipInputStream gzstream = new GZipInputStream(new FileStream( path, FileMode.Open));
		    TarExtracter untar = new TarExtracter();
		    untar.Extract(gzstream, dest_dir);
            gzstream.Close();
		}
	}
}
