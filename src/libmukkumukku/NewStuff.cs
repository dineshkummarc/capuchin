
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using NDesk.DBus;

namespace Nsm
{	
	[Interface("org.gnome.NewStuffManager.NewStuff")]
	public abstract class NewStuffProxy : MarshalByRefObject
	{
	    protected enum SignalType: uint {
	       Updated,
	       DownloadStatus,
	       Closed
	    }
	    
	    public delegate void UpdatedHandler(string plugin_id);
	    public delegate void DownloadStatusHandler(string action, double progress);
	    
		public event UpdatedHandler Updated;
	    public event DownloadStatusHandler DownloadStatus;
	    
	    internal delegate void ClosedHandler(string application_name);
	    internal event ClosedHandler Closed;
	    
	    public readonly string ApplicationName;
	    
	    public NewStuffProxy(string application_name)
	    {
	        this.ApplicationName = application_name;
	    }
	    
    	protected void InvokeSignal(SignalType s, params object[] args) {
			switch (s) {
				case SignalType.Updated:    	       
					if (Updated != null)
					{
						Updated( (string)args[0] );
					}
					break;
				case SignalType.DownloadStatus:
					if (DownloadStatus != null)
					{
						DownloadStatus((string)args[0], (double)args[1]);
					}
					break;
			    case SignalType.Closed:
			        if (Closed != null)
			        {
			            Closed(this.ApplicationName);
			        }
			        break;
			}
		}
	    
    	/// <summary>Update the plugin with ID <code>plugin_id</code></summary>
    	/// <param name="plugin_id">Plugin's ID</param>
    	public abstract void Update(string plugin_id);
    	
		/// <summary>Load the repository</summary>
    	public abstract void Refresh();
    	
		/// <summary>Get all plugins from the repository</summary>
		/// <returns>
		/// An array of string arrays of size 3.
		/// Whereas the first element is the plugin's id, second the plugin's name and third the description.
		/// </returns>
    	public abstract string[][] GetAvailableNewStuff();
    	
    	/// <summary>Get all available updates</summary>
    	/// <param name="plugins">
    	/// An array consisting of string arrays of size 2.
    	/// The first element is the plugin's id and the second its version.
    	/// </param>
    	/// <returns>An array of string arrays of size 2.
    	/// First element is the plugin's ID, second its description</returns>
    	public abstract string[][] GetAvailableUpdates(string[][] plugins);
    	
		/// <summary>Get tags for the plugin with ID <code>plugin_id</code></summary>
		/// <param name="plugin_id">Plugin's ID</param>
		/// <returns>An array of tags</returns>
    	public abstract string[] GetTags(string plugin_id);
    	
    	/// <summary>Get the author's name and e-mail address for the plugin with ID <code>plugin_id</code></summary>
    	/// <param name="plugin_id">Plugin's ID</param>
    	/// <returns>Dictionary with keys "name" and "email"</returns>
    	public abstract IDictionary<string, string> GetAuthor(string plugin_id);
    	
    	/// <summary>Tell the NewStuff object that it isn't needed anymore</summary>
    	public abstract void Close();
	}
	
	public class RepositoryConnectionException : ApplicationException
	{
		public RepositoryConnectionException() { }
		public RepositoryConnectionException(string message) : base(message) { }
		public RepositoryConnectionException(string message, Exception inner) : base(message, inner) { }
	}
	
	/// <summary>An application specific object that handels the plugins</summary>
	public class NewStuff : NewStuffProxy, IDisposable
	{
		public ItemsDict Repo;
		
		protected string SpecFile;
		protected bool ForceUpdate;
		protected string LocalRepo;
	    protected IDictionary<string, string> Options;
	    
	    private bool disposed = false;
	   
        public delegate void DownloaderDel();
	    
	    /// <param name="application_name">Name of the application that object is related to</param>
	    /// <remarks><code>application_name</code> must be the name of a configuration file without the .conf ending</remarks>
		public NewStuff(string application_name) : base(application_name)
		{
			Console.WriteLine("*** NewStuff init "+application_name);
			
			this.ForceUpdate = false;
			this.SpecFile = Path.Combine(Globals.SPECS_DIR, application_name+".conf");
			this.LocalRepo = Path.Combine(Globals.Instance.LOCAL_CACHE_DIR, application_name+".xml");
			
			ConfParser cp = new ConfParser(application_name);
			this.Options = cp.Options;
		}
		
		~NewStuff()
		{
		    this.Dispose();
		}
		
		public void Dispose()
		{
		    if (!this.disposed)
		    {
		        this.Options = null;
		        this.Repo = null;
		        this.disposed = true;
		        GC.SuppressFinalize(this);
		    }
		}
		
		public override void Refresh()
		{
			Console.WriteLine("*** Refreshing");			
			
			if (this.ForceUpdate || !this.IsCacheUpToDate())
			{
				Console.WriteLine("*** Downloading XML file");
				using (WebClient wc = new WebClient())
				{
					wc.DownloadFile( this.Options["repo"], this.LocalRepo );
				}
			}
			
			Console.WriteLine("*** Deserializing");
			XmlSerializer ser = new XmlSerializer(typeof(ItemsDict));
			
			FileStream repo_stream = new FileStream( this.LocalRepo, FileMode.Open );
		    /*
            NameTable nt = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
		    XmlParserContext context = new XmlParserContext(null, nsmgr, "de", XmlSpace.None);
		    XmlTextReader reader = new XmlTextReader(repo_stream, XmlNodeType.Document, context);
		    */
		    XmlTextReader reader = new XmlTextReader(repo_stream);
		    this.Repo = (ItemsDict)ser.Deserialize( reader );
		    reader.Close();
		}
		
    	public override string[][] GetAvailableNewStuff()
    	{
    		Console.WriteLine("*** Getting available Stuff");
    		
    		string[][] stuff = new string[this.Repo.Count][];
    		int c=0;
    		foreach (string id in this.Repo.Keys)
    		{
    			stuff[c] = new string[] { id, this.Repo[id].Name, this.Repo[id].Description };
    			c++;
    		}
    		
    		return stuff;
    	}
    	
    	public override string[][] GetAvailableUpdates(string[][] plugins)
    	{
            Console.WriteLine("*** Getting updates");
            
            List<string[]> updates = new List<string[]>();
    	    foreach (string[] p in plugins) {
    	        string plugin_id = p[0];
                if (!this.Repo.ContainsKey(plugin_id))
                    continue;
                
                string repo_version = this.Repo[plugin_id].Version;
                if (NewStuff.IsNewerVersion(repo_version, p[1]))
                {
                    updates.Add( new string[] {plugin_id, this.Repo[plugin_id].Description } );
                }
	    	}
	    	 
    		return updates.ToArray();
    	}    	
    	
		public override void Update(string plugin_id)
		{
		    if (!this.Repo.ContainsKey(plugin_id))
		        return;
			Console.WriteLine("*** Updating {0}", plugin_id);            
            
            Downloader dl = new Downloader(this.Repo[plugin_id].Url, this.Options["install-path"], this.Repo[plugin_id].Signature, this.Repo[plugin_id].Checksum);
			dl.Status += new StatusHandler(
			 delegate(string action, double progress) { InvokeSignal(SignalType.DownloadStatus, action, progress); }
			);
			
			DownloaderDel dlDel = new DownloaderDel(dl.Run);
            AsyncCallback dlCallback = new AsyncCallback(DownloaderFinishedCallback);
            dlDel.BeginInvoke(dlCallback, plugin_id);
		}
		
    	public override string[] GetTags(string plugin_id)
    	{
            Console.WriteLine("*** Getting tags for {0}", plugin_id);
            string[] tags = this.Repo[plugin_id].Tags;
            return (tags == null) ? new string[] {""} : tags;
    	}
    	
    	public override IDictionary<string, string> GetAuthor(string plugin_id)
    	{
            Console.WriteLine("*** Getting author for {0}", plugin_id);
            return this.Repo[plugin_id].Author;
    	}
    	
    	public override void Close()
    	{
    		Console.WriteLine("*** Closing");
    		InvokeSignal(SignalType.Closed);
    	}
    	
    	/// <summary>Callback function to async <see cref="Downloader" /> Thread</summary>
    	private void DownloaderFinishedCallback(IAsyncResult ar) {
            Console.WriteLine("*** Update complete for {0}", (string)ar.AsyncState);    	   
            InvokeSignal(SignalType.Updated, (string)ar.AsyncState);
    	}
    	
    	/// <summary>Check whether repository's XML file has to be downloaded again</summary>
    	/// <exception cref="Nsm.RepositoryConnectionException">
    	/// Thrown if connection to repository failed
    	/// </exception>
    	private bool IsCacheUpToDate()
    	{
    		if (!File.Exists(this.LocalRepo))
    			return false;
    			
    		try {
    			HttpWebRequest wr = (HttpWebRequest)WebRequest.Create( this.Options["repo"] );
        		HttpWebResponse wresp = (HttpWebResponse)wr.GetResponse();
        		DateTime remoteModTime = wresp.LastModified;
        		wresp.Close();
        		return (File.GetLastWriteTime(this.LocalRepo) >= remoteModTime);
        	} catch (WebException e) {
        		throw new RepositoryConnectionException("Connection to repository "+this.Options["repo"]+" failed", e);
        	}
    	}
    	
    	/// <summary>Check whether first argument is newer as second one</summary>
    	private static bool IsNewerVersion(string version_new, string version_old)
        {
        	int[] new_arr = NewStuff.ConvertVersionStringToIntArray(version_new);
        	int[] old_arr = NewStuff.ConvertVersionStringToIntArray(version_old);
        	
        	if (new_arr.Length < 4) Array.Resize<int>(ref new_arr, 4);
        	if (old_arr.Length < 4) Array.Resize<int>(ref old_arr, 4);
        	
        	if ( new_arr[0] > old_arr[0]) return true;
        	else if ( new_arr[1] > old_arr[1] ) return true;
        	else if ( new_arr[2] > old_arr[2] ) return true;
        	else if ( new_arr[3] > old_arr[3] ) return true;
        	else if ( new_arr[0] == old_arr[0] && (new_arr[1] > new_arr[1] || new_arr[2] > old_arr[2] || new_arr[3] > old_arr[3]) ) return true;
        	else if ( new_arr[1] == old_arr[1] && (new_arr[2] > old_arr[2]  || new_arr[3] > old_arr[3] ) ) return true;
        	else if ( new_arr[2] == old_arr[2] && new_arr[3] > old_arr[3] ) return true;
        	else return false;
        }
        
        /// <summary>Convert a version string to an int array</summary>
        private static int[] ConvertVersionStringToIntArray(string version)
        {
        	string[] string_arr = version.Split(new char[] {'.'});
        	int[] int_arr = new int[string_arr.Length];
        	for (int i=0; i<string_arr.Length; i++)
        	{
        		int_arr[i] = Convert.ToInt32(string_arr[i]);
        	}
        	return int_arr;
        }
	}
}
