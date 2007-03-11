
using System;
using System.Collections.Generic;
using System.IO;

namespace Nsm
{
	
	public class ParsingConfException : ApplicationException
	{
		public ParsingConfException() { }
		public ParsingConfException(string message) : base(message) { }
		public ParsingConfException(string message, Exception inner) : base(message, inner) { }
	}
	
	/// <summary>Parses a configuration file</summary>
	internal class ConfParser
	{
		private IDictionary<string,string> optionsField;
	    
	    /// <value>The options saved in the configuration file</value>
		public IDictionary<string,string> Options
		{
			get
			{
				return this.optionsField;
			}
		}
		
		/// <param name="application_name">
		/// The name of the configuration file without the .conf ending
		/// </param>
		/// <exception cref="Nsm.ParsingConfException">
		/// Thrown when something went wrong while parsing repository's config file
		/// </exception>
		public ConfParser(string application_name)
		{
			this.optionsField = new Dictionary<string, string>();
			string conffile = Path.Combine(Globals.SPECS_DIR, application_name+".conf");
			
			try {
				using (TextReader reader = new StreamReader(conffile))
				{				
					string line;
					bool in_section = false;
					
					while ((line = reader.ReadLine()) != null)
					{
						line = line.Trim();
						if (line.StartsWith("#") || line.StartsWith(";")) continue;			
						
						if (line == "["+ application_name + "]")
						{
							in_section = true;
							continue;
						}
						
						if (in_section)
						{
							string[] cols = line.Split(new char[] {'='}, 2);
							for(int i=0; i<cols.Length; i++)
							{
								cols[i] = cols[i].Trim();
							}
							
							if (cols.Length == 2)
							{
								this.optionsField.Add(cols[0], cols[1]);
							}
						}
					}
				}
			} catch (IOException e) {
				throw new ParsingConfException("Error while reading config file "+conffile, e);
			}
			
			// Checking for required fields
			if (!(this.Options.ContainsKey("install-path") && this.Options.ContainsKey("repo")))
			{
				throw new ParsingConfException("Repository's config file must contain values for `install-path' and `repo'");
			}
			
			this.optionsField["install-path"] = ConfParser.ExpandPath(this.Options["install-path"]);
			
		}
		
		/// <summary>Substitute a starting `~' with the user's home directory</summary>
		/// <param name="path">The path to expand</param>
		private static string ExpandPath(string path)
        {
            if (path.StartsWith("~"))
            {
                string home_dir = Environment.GetFolderPath( Environment.SpecialFolder.Personal );
                path = path.Replace("~", home_dir);   
            }
            return path;
        }		
	}
}
