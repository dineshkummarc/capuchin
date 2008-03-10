using System;

namespace Capuchin.Installation
{
	
	public interface IInstaller
	{
		bool CanInstallFile (string location);
		void InstallFile (string location);
	}
}
