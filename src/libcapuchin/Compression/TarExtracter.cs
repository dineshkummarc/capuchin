
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Tar;

namespace Capuchin.Compression
{
    
    
    internal class TarExtracter : IExtracter
    {
        
        public void Extract(string path, string dest_dir)
        {
            Stream fstream = new FileStream(path, FileMode.Open);
            this.Extract(fstream, dest_dir);
            fstream.Close();
        }
        
        public void Extract(Stream stream, string dest_dir)
        {
          TarArchive archive = TarArchive.CreateInputTarArchive(stream);
          //archive.ProgressMessageEvent += new ProgressMessageHandler(OnProgress);
          archive.ExtractContents( dest_dir );
          archive.CloseArchive();
        }
    }
}
