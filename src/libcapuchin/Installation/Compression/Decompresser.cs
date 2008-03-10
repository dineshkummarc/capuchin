
using System;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using Capuchin.Logging;

namespace Capuchin.Installation.Compression
{
    internal class Decompresser
    {
        private string path;
        private string dest_dir;
        private bool deleteField;
        
        public bool DeleteFile
        {
            get { return this.deleteField; }
        }
        
        public Decompresser(string path, string dest_dir)
        {
            this.path = path;
            this.dest_dir = dest_dir;
            this.deleteField = false;
        }
        
        public void Run() {
			Log.Info("Decompressing {0} to {1}", this.path, this.dest_dir);
			
			string mime_type = Gnome.Vfs.MimeType.GetMimeTypeForUri(path);

			IExtracter extracter = null;
			if (mime_type == "application/x-bzip-compressed-tar"){
				extracter = new TarBz2Extracter();
				this.deleteField = true;
			} else if (mime_type == "application/x-compressed-tar") {
				extracter = new TarGzExtracter();
				this.deleteField = true;
			} else if (mime_type == "application/zip") {
				extracter = new ZipExtracter();
				this.deleteField = true;
			}
			// Do nothing for "text/x-python"
			if (extracter != null)
				extracter.Extract(this.path, this.dest_dir);
			}
    }
    
}
