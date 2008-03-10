
using System;

namespace Capuchin.Installation.Compression
{
    internal interface IExtracter
    {
       void Extract(string path, string dest_dir);
    }
}
