
using System;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace Capuchin.Verification
{
    
    
    internal class GnuPGVerifier : IVerifier
    {
        private bool IsValidField;
        
        protected string sigUrl;
        protected string localFile;        
        protected const string CMD_ARGS = "--keyserver=hkp://subkeys.pgp.net --keyserver-options=auto-key-retrieve --verify {0} {1}";
        protected Process gpg;
        
        public bool IsValid
        {
            get { return this.IsValidField; }
        }
        
        public GnuPGVerifier(string file_path, string sig_url)
        {
            this.sigUrl = sig_url;
            this.localFile = Path.Combine( Globals.Instance.LOCAL_CACHE_DIR, Path.GetFileName(sig_url) );
            
            this.gpg = new Process();
            this.gpg.StartInfo.FileName = Globals.GPG_BIN;
            this.gpg.StartInfo.Arguments = String.Format( CMD_ARGS, this.localFile, file_path );
            
            try {
                this.DownloadSignature();                
                this.gpg.Start();
                this.gpg.WaitForExit();
            } finally {
                this.DeleteSignature();
            }
            this.IsValidField = (gpg.ExitCode == 0);
        }
        
        private void DownloadSignature()
        {
           WebClient wc = new WebClient();
           wc.DownloadFile(this.sigUrl, this.localFile);
        }
        
        private void DeleteSignature()
        {
            File.Delete(this.localFile);
        }
    }
}
