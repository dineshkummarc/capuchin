
using System;
using System.IO;
using ICSharpCode.SharpZipLib.BZip2;

namespace Capuchin.Compression
{
    
    
    internal class TarBz2Extracter : IExtracter
    {
        public void Extract(string path, string dest_dir)
        {
            BZip2InputStream bz2stream = new BZip2InputStream(new FileStream( path, FileMode.Open));
            TarExtracter untar = new TarExtracter();
            untar.Extract(bz2stream, dest_dir);
            bz2stream.Close();
        }
    }
}
