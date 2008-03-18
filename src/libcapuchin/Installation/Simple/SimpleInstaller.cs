using System;
using System.IO;
using Capuchin.Logging;
using Capuchin.Installation;
using Capuchin.Installation.Compression;

namespace Capuchin.Installation.Simple
{
	/// <summary>
	/// Just copies the file to the desired location
	/// or extracts the archive (if applicable)
	/// </summary>
	public class SimpleInstaller : IInstaller
	{
		protected readonly string InstallPath; 
		
		/// <summary>
		/// Creates a new class
		/// </summary>
		/// <param name="install_dir">Directory where the files will be installed</param>
		public SimpleInstaller(string install_dir)
		{
			this.InstallPath = install_dir;
		}
		
		/// <summary>
		/// Whether the <see cref="Capuchin.Installation.IInstaller"/>
		/// can install the provided file
		/// </summary>
		/// <param name="location">Where the file to install is located</param>
		/// <returns>
		/// Always true. As this <see cref="Capuchin.Installation.IInstaller"/>
		/// just copies the file or extracts the archive (if applicable).
		/// </returns>		
		public bool CanInstallFile (string location)
		{
			return true;
		}
		
		public void InstallFile (string location)
		{
			Log.Info ("Installing file {0} to {1}", location, this.InstallPath);
			
			Decompresser decomp = new Decompresser(location, this.InstallPath);
            decomp.Run();
         
            if (decomp.DeleteFile) {
                Log.Info("Deleting archive {0}", location);
				File.Delete(location);
			}
		}
	}
}
