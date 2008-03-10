using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using Capuchin.Verification;
using Capuchin.Installation;
using Capuchin.Installation.Simple;
using Capuchin.Logging;
using Capuchin.Xml;

namespace Capuchin
{
    
    public class RepositoryConnectionException : ApplicationException
    {
        public RepositoryConnectionException() { }
        public RepositoryConnectionException(string message) : base(message) { }
        public RepositoryConnectionException(string message, Exception inner) : base(message, inner) { }
    }
    
    /// <summary>
	/// An application specific object that handels the plugins
	/// </summary>
    public class AppObject : IDisposable, IAppObject
    {
        public event UpdateFinishedHandler UpdateFinished;
        public event InstallFinishedHandler InstallFinished;
        public event StatusHandler Status;
        
        internal delegate void ClosedHandler(string application_name);
        internal event ClosedHandler Closed;
    
        internal ItemsDict RepoItems;
        public readonly string RepositoryURL;
        
        protected string LocalRepo;
        protected string InstallPath;
        protected IDictionary<int, string> DownloadToPluginId;
        protected string ApplicationName;
		protected IDictionary<string, List<string>> TagToPlugins;
        
        private const int SLEEP_TIME = 500;
        private bool disposed = false;
        private int repo_dlid = -1; 
        
        /// <param name="repository_url">URL to repository's XML file</param>
        public AppObject (string repository_url)
        {
            this.RepositoryURL = repository_url;
            this.LocalRepo = Path.Combine(Globals.Instance.LOCAL_CACHE_DIR, Path.GetFileName(repository_url));
            // Used to map DownloadId to PluginID
            this.DownloadToPluginId = new Dictionary<int, string>();
			// Used to map tag to plugin ids
			this.TagToPlugins = new Dictionary<string, List<string>> ();
            
            // Forward DownloadStatus event
            Globals.DLM.DownloadStatus += new DownloadManagerStatusHandler(
                    this.OnDownloadStatus
            );
            Globals.DLM.DownloadFinished += new DownloadManagerFinishedHandler(
                this.OnDownloadFinished
            );
        }
            
        ~AppObject()
        {
            this.Dispose();
        }
        
        public void Dispose()
        {
            if (!this.disposed)
            {
                // Forward DownloadStatus event
                Globals.DLM.DownloadStatus -= new DownloadManagerStatusHandler(
                        this.OnDownloadStatus
                );
                Globals.DLM.DownloadFinished -= new DownloadManagerFinishedHandler(
                    this.OnDownloadFinished
                );                
                
                this.RepoItems = null;
                this.DownloadToPluginId = null;
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        
        /// <summary>Load the repository</summary>
        /// <param name="force_update">Force downloading repository's XML file</param>
        public void Update (bool force_update)
        {
            Log.Info("Refreshing");            
            
            if (force_update || !this.IsCacheUpToDate())
            {
                Log.Info("Downloading XML file from {0}", this.RepositoryURL);
                File.Delete( this.LocalRepo );
                
                this.repo_dlid = Globals.DLM.DownloadFile (this.RepositoryURL, Globals.Instance.LOCAL_CACHE_DIR);
            } else {
                this.LoadRepository ();
            }
        }
        
        protected void LoadRepository ()
        {
            Log.Info("Deserializing XML file");
            XmlSerializer ser = new XmlSerializer(typeof(Repository));
            
            FileStream repo_stream = new FileStream( this.LocalRepo, FileMode.Open );
            /*
            NameTable nt = new NameTable();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nt);
            XmlParserContext context = new XmlParserContext(null, nsmgr, "de", XmlSpace.None);
            XmlTextReader reader = new XmlTextReader(repo_stream, XmlNodeType.Document, context);
            */
            XmlTextReader reader = new XmlTextReader(repo_stream);
            Repository repo = (Repository)ser.Deserialize( reader );
            reader.Close();
            this.RepoItems = repo.items;
            this.InstallPath = ExpandPath(repo.installpath);
            this.ApplicationName = repo.application;
			
			this.fillTagToPlugins ();
            
            this.OnUpdateFinished();
        }
        
        public string GetApplicationName ()
        {
            return this.ApplicationName;
        }       
        
        /// <summary>Get all plugins from the repository</summary>
        /// <returns>
        /// An array of string arrays of size 2.
        /// Whereas the first element is the plugin's id, second the plugin's name.
        /// </returns>
        public string[] GetAvailablePlugins ()
        {
            Log.Info("Getting available plugins");
            
            string[] ids = new string[this.RepoItems.Count];
            int c=0;
            foreach (string id in this.RepoItems.Keys)
            {
                ids[c] = id;
                c++;
            }
            
            return ids;
        }
        
        /// <summary>Get all available updates</summary>
        /// <param name="plugins">
        /// An array consisting of string arrays of size 2.
        /// The first element is the plugin's id and the second its version.
        /// </param>
        /// <returns>An array of strings containing plugin IDs</returns>
        public string[] GetAvailableUpdates (string[][] plugins)
        {
            Log.Info("Getting updates");
            
            List<string> updates = new List<string>();
            foreach (string[] p in plugins) {
                string plugin_id = p[0];
                if (!this.RepoItems.ContainsKey(plugin_id))
                    continue;
                
                string repo_version = this.RepoItems[plugin_id].Version;
                if (AppObject.IsNewerVersion(repo_version, p[1]))
                {
                    updates.Add (plugin_id);
                }
            }
             
            return updates.ToArray();
        }        
        
        /// <summary>
        /// Update the plugin with ID <code>plugin_id</code>
        /// </summary>
        /// <param name="plugin_id">Plugin's ID</param>
        public void Install (string plugin_id)
        {
            if (!this.RepoItems.ContainsKey(plugin_id))
                return;
            Log.Info("Updating plugin with id '{0}'", plugin_id);
            
            int dlid = Globals.DLM.DownloadFile(this.RepoItems[plugin_id].Url, this.InstallPath, this.RepoItems[plugin_id].Signature, this.RepoItems[plugin_id].Checksum);
            
            lock (this) {
                this.DownloadToPluginId.Add(dlid, plugin_id);
            }
        }
        
        /// <summary>
        /// Returns all plugins that are tagged with the given tag.
        /// The method works case in-sensitive.
        /// </summary>
        /// <param name="tag">A tag</param>
        /// <returns>A list of plugin IDs</returns>
        public string[] GetPluginsWithTag (string tag)
        {
			tag = tag.Trim().ToLower ();
			if (!this.TagToPlugins.ContainsKey (tag)) {
				return new string[] {};
			}
			
			List<string> pluginList = this.TagToPlugins[tag];
			string[] plugins = new string[pluginList.Count];
			pluginList.CopyTo (plugins, 0);
            return plugins;
        }
        
        /// <summary>
        /// Get name of plugin with given <code>plugin_id</code>
        /// </summary>
        public string GetPluginName (string plugin_id)
        {
            return this.RepoItems[plugin_id].Name;
        }
        
        /// <summary>
        /// Get description for given <code>plugin_id</code>
        /// </summary>
        public string GetPluginDescription (string plugin_id)
        {
            return this.RepoItems[plugin_id].Description;   
        }
        
        /// <summary>
        /// Get changes for plugin with given ID made in given version
        /// </summary>
        public string GetPluginChanges (string plugin_id, string version)
        {
			changelog changes = this.RepoItems[plugin_id].Changelog;
            if (changes != null && changes.ContainsKey (version))
            {
                return this.RepoItems[plugin_id].Changelog[version];
            } else {
                return String.Empty;
            }
        }
        
        /// <summary>
        /// Get tags for the plugin with ID <code>plugin_id</code>
        /// </summary>
        /// <param name="plugin_id">Plugin's ID</param>
        /// <returns>An array of tags</returns>
        public string[] GetPluginTags (string plugin_id)
        {
            Log.Info("Getting tags for plugin with id '{0}'", plugin_id);
            string[] tags = this.RepoItems[plugin_id].Tags;
            return (tags == null) ? new string[] {} : tags;
        }
        
        /// <summary>
        /// Get the author's name and e-mail address for the plugin with ID <code>plugin_id</code>
        /// </summary>
        /// <param name="plugin_id">Plugin's ID</param>
        /// <returns>Dictionary with keys "name" and "email"</returns>
        public string[] GetPluginAuthor (string plugin_id)
        {
            Log.Info("Getting author of plugin with id '{0}'", plugin_id);
            
            author plugin_author = this.RepoItems[plugin_id].Author;
            return new string[] { plugin_author.Name, plugin_author.Email };
        }
        
		/// <summary>
		/// Get all available tags available in this repository
		/// </summary>
		/// <returns>A list of tags</returns>
        public string[] GetTags ()
        {
			ICollection<string> tagsCol = this.TagToPlugins.Keys;
			string[] tags = new string[tagsCol.Count];
			tagsCol.CopyTo (tags, 0);
            return tags;
        }
        
        /// <summary>Tell the object that it isn't needed anymore</summary>
        public void Close()
        {
            Log.Info("Closing object for {0}", this.ApplicationName);
            this.Dispose();
            this.OnClosed();
        }
        
        /// <summary>Check checksum and signature of the file</summary>
        /// <param name="local_file">Path to the downloaded file</param>
        /// <param name="signature">URL of the signature file</param>
        /// <param name="checksumField">Checksum information</param>
        internal void CheckFile (string local_file, string signature, checksum checksumField)
        {
			Log.Info ("Checking file");
			
            if (checksumField != null)
            {
                FileStream fs = new FileStream( local_file, FileMode.Open );
                ChecksumVerifier cv = new ChecksumVerifier( fs, checksumField );
                fs.Close();
                
                Log.Info("Checksum valid for {0}: {1}", local_file, cv.IsValid);
                if (!cv.IsValid)
                {
                    // Checksum is invalid
                    File.Delete(local_file);
                    return;
                }
            }
            
            if (signature != null)
            {
                GnuPGVerifier gv = new GnuPGVerifier(local_file, signature);
                
                Log.Info("Signature valid for {0}: {1}", local_file, gv.IsValid);
                if (!gv.IsValid)
                {
                    // Signature invalid
                    File.Delete(local_file);
                    return;
                }
            }
        }
         
        /// <summary>Actually install the file</summary>
        /// <param name="local_file_obj">
		/// A <see cref="System.String" /> where the file is located
		/// </param>   
        protected void InstallFileReal (object local_file_obj)
        {   
			Log.Info ("Installing file");
			
            string local_file = (string)local_file_obj;

			IInstaller installer = new SimpleInstaller (this.InstallPath);
			installer.InstallFile (local_file);
        }
		
		protected void fillTagToPlugins ()
		{
			this.TagToPlugins.Clear ();
			
			foreach (item itemEntry in this.RepoItems.Values)
			{
				this.addPluginToTags (itemEntry);
			}
		}
        
		protected void addPluginToTags (item pluginItem)
		{
			if (pluginItem.Tags == null) return;
			
			foreach (string tag in pluginItem.Tags)
			{
				string lowertag = tag.Trim().ToLower ();
				// Check if list for tag already exists
				if (!this.TagToPlugins.ContainsKey (lowertag)) 
				{
					// Create new List
					this.TagToPlugins.Add (lowertag, new List<string> ());
				}
				// Add plugin's id to list
				this.TagToPlugins[lowertag].Add (pluginItem.Id);
			}
		}
		
        protected void OnUpdateFinished ()
        {
            if (UpdateFinished != null)
            {
                UpdateFinished ();
            }
        }
        
        protected void OnInstallFinished (string id)
        {
            if (InstallFinished != null)
            {
                InstallFinished( id );            
            }
        }
        
        protected void OnStatus (ActionType action, string plugin_id, double progress, int speed)
        {
            if (Status != null)
            {
                Status (action, plugin_id, progress, speed);
            }
        }
        
        protected void OnClosed ()
        {
            if (Closed != null)
            {
                Closed(this.RepositoryURL);
            }
        }
        
        /// <summary>
        /// Check whether repository's XML file has to be downloaded again
        /// </summary>
        /// <exception cref="Capuchin.RepositoryConnectionException">
        /// Thrown if connection to repository failed
        /// </exception>
        private bool IsCacheUpToDate ()
        {
            Log.Info("Checking if cache is up to date");            
            
            if (!File.Exists(this.LocalRepo))
                return false;
                
            try {
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create( this.RepositoryURL );
                HttpWebResponse wresp = (HttpWebResponse)wr.GetResponse();
                DateTime remoteModTime = wresp.LastModified;
                wresp.Close();
                return (File.GetLastWriteTime(this.LocalRepo) >= remoteModTime);
            } catch (WebException e) {
				throw new RepositoryConnectionException("Connection to repository "+this.RepositoryURL+" failed: "+e.Message, e);
            }
        }
            
        private void OnDownloadStatus (int dlid, double progress, int speed)
        {
            if (dlid == this.repo_dlid)
            {
                this.OnStatus (ActionType.UpdatingRepo, "", progress, speed);
            } else {
                lock (this) {
                    if (!this.DownloadToPluginId.ContainsKey(dlid))
                        throw new ArgumentException ("Could not find download with id "+dlid);
                    
                    this.OnStatus (ActionType.DownloadingPlugin, this.DownloadToPluginId[dlid], progress, speed);
                }
            }
        }
        
        private void OnDownloadFinished (int dlid)
        {
			Log.Info ("DA SAMER");
            if (dlid == this.repo_dlid) {
                this.LoadRepository();
                return;
            }
            
            string plugin_id = null;
            lock (this) {
                if (!this.DownloadToPluginId.ContainsKey(dlid))
                    throw new ArgumentException ("Could not find download with id "+dlid);            
                    
               plugin_id = this.DownloadToPluginId[dlid];
            }
            string local_file = Path.Combine( this.InstallPath, Path.GetFileName(this.RepoItems[plugin_id].Url) );
        
            // Check file
            this.CheckFile(local_file, this.RepoItems[plugin_id].Signature, this.RepoItems[plugin_id].Checksum);
            
            // Install file
            Thread installThread = new Thread( new ParameterizedThreadStart( this.InstallFileReal ) );
			installThread.Start( local_file );
            while (installThread.IsAlive)
            {
                this.OnStatus( ActionType.ExtractingPlugin, plugin_id, -1.0, -1);
                Thread.Sleep(SLEEP_TIME);
            }
            
            Log.Info("Updated plugin with id '{0}'", plugin_id);

            this.OnInstallFinished(plugin_id);
            lock (this) {
                this.DownloadToPluginId.Remove(dlid);
            }
        }
        
        /// <summary>Check whether first argument is newer as second one</summary>
        private static bool IsNewerVersion (string version_new, string version_old)
        {
            int[] new_arr = AppObject.ConvertVersionStringToIntArray(version_new);
            int[] old_arr = AppObject.ConvertVersionStringToIntArray(version_old);
            
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
        private static int[] ConvertVersionStringToIntArray (string version)
        {
            string[] string_arr = version.Split(new char[] {'.'});
            int[] int_arr = new int[string_arr.Length];
            for (int i=0; i<string_arr.Length; i++)
            {
                int_arr[i] = Convert.ToInt32(string_arr[i]);
            }
            return int_arr;
        }        
        
        private static string ExpandPath (string path)
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
