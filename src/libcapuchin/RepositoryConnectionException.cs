using System;

namespace Capuchin
{
	
    public class RepositoryConnectionException : ApplicationException
    {
        public RepositoryConnectionException() { }
        public RepositoryConnectionException(string message) : base(message) { }
        public RepositoryConnectionException(string message, Exception inner) : base(message, inner) { }
    }
	
}
