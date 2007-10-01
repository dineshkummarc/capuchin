
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Capuchin.Compression
{
	
	internal class ZipExtracter : IExtracter
	{
		
		public void Extract(string path, string dest_dir)
		{
		  ZipInputStream zipstream = new ZipInputStream(new FileStream( path, FileMode.Open));
		  
		  ZipEntry theEntry;
		  while ((theEntry = zipstream.GetNextEntry()) != null) {
            string directoryName = Path.GetDirectoryName(theEntry.Name);
			string fileName      = Path.GetFileName(theEntry.Name);
			
			// create directory
			if (directoryName != String.Empty)
			    Directory.CreateDirectory(directoryName);
			
			if (fileName != String.Empty) {
				FileStream streamWriter = File.Create( Path.Combine( dest_dir, theEntry.Name) );
				
				int size = 0;
				byte[] buffer = new byte[2048];
				while ((size = zipstream.Read(buffer, 0, buffer.Length)) > 0) {					
				    streamWriter.Write(buffer, 0, size);
				}
				streamWriter.Close();
			}
		  }
		  zipstream.Close();
		}
	}
}
