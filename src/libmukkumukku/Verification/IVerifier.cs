
using System;

namespace Nsm.Verification
{
	
	
	internal interface IVerifier
	{
	    /// <value>Whether verification succeded or not</value>
	    bool IsValid { get; }
	}
}
