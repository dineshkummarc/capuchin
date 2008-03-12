using System;

namespace Capuchin
{
	
    public class RepositoryNotInitializedException : ApplicationException
    {
        public RepositoryNotInitializedException() { }
        public RepositoryNotInitializedException(string message) : base(message) { }
        public RepositoryNotInitializedException(string message, Exception inner) : base(message, inner) { }
    }
	
}