
using System;

namespace Capuchin.Compression
{
    internal interface IExtracter
    {
       void Extract(string path, string dest_dir);
    }
}
