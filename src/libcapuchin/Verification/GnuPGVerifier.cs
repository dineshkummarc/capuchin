/*
 * This file is part of Capuchin
 * 
 * Copyright (C) Sebastian Pölsterl 2008 <marduk@k-d-w.org>
 * 
 * Capuchin is free software.
 * 
 * You may redistribute it and/or modify it under the terms of the
 * GNU General Public License, as published by the Free Software
 * Foundation; either version 2 of the License, or (at your option)
 * any later version.
 * 
 * Capuchin is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Capuchin.  If not, write to:
 * 	The Free Software Foundation, Inc.,
 * 	51 Franklin Street, Fifth Floor
 * 	Boston, MA  02110-1301, USA.
 */

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
