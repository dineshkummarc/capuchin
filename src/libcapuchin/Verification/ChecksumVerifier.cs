
using System;
using System.Security.Cryptography;

namespace Capuchin.Verification
{
    
    internal class ChecksumVerifier : IVerifier
    {
        private bool IsValidField;
        protected HashAlgorithm uncrypt;
        
        public bool IsValid
        {
            get { return this.IsValidField; }
        }
        
        public ChecksumVerifier(System.IO.Stream stream, checksum checksumField)
        {
            switch (checksumField.type)
            {
                case (checksumType.md5):
                uncrypt = new MD5CryptoServiceProvider();                
                break;
                
                case (checksumType.sha1):
                uncrypt = new SHA1CryptoServiceProvider();
                break;
            }
            
            byte[] result = uncrypt.ComputeHash(stream);
            string computedChecksum = BitConverter.ToString(result).Replace("-", "").ToLower();
            
            this.IsValidField = (computedChecksum == checksumField.Text);
        }
    }
}
