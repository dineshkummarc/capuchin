using System;

namespace Capuchin
{
	
	public class NoSuchPluginException : ApplicationException
    {
        public NoSuchPluginException() { }
        public NoSuchPluginException(string message) : base(message) { }
        public NoSuchPluginException(string message, Exception inner) : base(message, inner) { }
    }
    
}
