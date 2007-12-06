
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using NDesk.DBus;
using Capuchin.Verification;
using Capuchin.Compression;
using Capuchin.Logging;

namespace Capuchin
{
    public enum ActionType {
        UpdatingRepo,
        DownloadingPlugin,
        ExtractingPlugin
    }
    
    public delegate void UpdateFinishedHandler ();
    public delegate void InstallFinishedHandler (string plugin_id);
    public delegate void StatusHandler (ActionType action, string plugin_id, double progress, int speed);
            
    [Interface("org.gnome.Capuchin.AppObject")]
    public interface IAppObject
    {
        event UpdateFinishedHandler UpdateFinished;
        event InstallFinishedHandler InstallFinished;
        event StatusHandler Status;
        
        string GetApplicationName ();
        void Update (bool force_update);
        void Install (string plugin_id);
        string[][] GetAvailablePlugins ();        
        string[] GetAvailableUpdates (string[][] plugins);
        string GetDescription (string plugin_id);
        string GetChanges (string plugin_id, string version);
        string[] GetTags (string plugin_id);        
        IDictionary<string, string> GetAuthor (string plugin_id);        
        void Close ();
    }
    
    public class RepositoryConnectionException : ApplicationException
    {
        public RepositoryConnectionException() { }
        public RepositoryConnectionException(string message) : base(message) { }
        public RepositoryConnectionException(string message, Exception inner) : base(message, inner) { }
    }
    
    /// <summary>An application specific object that handels the plugins</summary>
    public class AppObject : IDisposable,IAppObject
    {
        public event UpdateFinishedHandler UpdateFinished;
        public event InstallFinishedHandler InstallFinished;
        public event StatusHandler Status;
        
        internal delegate void ClosedHandler(string application_name);
        internal event ClosedHandler Closed;
    
        public ItemsDict RepoItems;
        public readonly string RepositoryURL;
        
        protected string LocalRepo;
        protected string InstallPath;
        protected IDictionary<int, string> DownloadToPluginId;
        protected string ApplicationName;
        
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
                this.RepoItems = null;
                this.disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        
        /// <summary>Load the repository</summary>
        /// <param name="force_update">Force downloading reository's XML file</param>
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
        public string[][] GetAvailablePlugins ()
        {
            Log.Info("Getting available plugins");
            
            string[][] stuff = new string[this.RepoItems.Count][];
            int c=0;
            foreach (string id in this.RepoItems.Keys)
            {
                stuff[c] = new string[] { id, this.RepoItems[id].Name };
                c++;
            }
            
            return stuff;
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
        
        /// <summary>Update the plugin with ID <code>plugin_id</code></summary>
        /// <param name="plugin_id">Plugin's ID</param>
        public void Install (string plugin_id)
        {
            if (!this.RepoItems.ContainsKey(plugin_id))
                return;
            Log.Info("Updating plugin with id '{0}'", plugin_id);
            
            int dlid = Globals.DLM.DownloadFile(this.RepoItems[plugin_id].Url, this.InstallPath, this.RepoItems[plugin_id].Signature, this.RepoItems[plugin_id].Checksum);
            
            this.DownloadToPluginId.Add(dlid, plugin_id);
        }
        
        /// <summary>
        /// Get description for given <code>plugin_id</code>
        /// </summary>
        public string GetDescription (string plugin_id)
        {
            return this.RepoItems[plugin_id].Description;   
        }
        
        /// <summary>
        /// Get changes for plugin with given ID made in given version
        /// </summary>
        public string GetChanges (string plugin_id, string version)
        {
            if (this.RepoItems[plugin_id].Changelog.ContainsKey (version))
            {
                return this.RepoItems[plugin_id].Changelog[version];
            } else {
                return "";
            }
        }
        
        /// <summary>Get tags for the plugin with ID <code>plugin_id</code></summary>
        /// <param name="plugin_id">Plugin's ID</param>
        /// <returns>An array of tags</returns>
        public string[] GetTags (string plugin_id)
        {
            Log.Info("Getting tags for plugin with id '{0}'", plugin_id);
            string[] tags = this.RepoItems[plugin_id].Tags;
            return (tags == null) ? new string[] {""} : tags;
        }
        
        /// <summary>Get the author's name and e-mail address for the plugin with ID <code>plugin_id</code></summary>
        /// <param name="plugin_id">Plugin's ID</param>
        /// <returns>Dictionary with keys "name" and "email"</returns>
        public IDictionary<string, string> GetAuthor (string plugin_id)
        {
            Log.Info("Getting author of plugin with id '{0}'", plugin_id);
            return this.RepoItems[plugin_id].Author;
        }
        
        /// <summary>Tell the object that it isn't needed anymore</summary>
        public void Close()
        {
            Log.Info("Closing object for {0}", this.ApplicationName);
            this.OnClosed();
        }
        
        /// <summary>Check checksum and signature of the file</summary>
        /// <param name="local_file">Path to the downloaded file</param>
        /// <param name="signature">URL of the signature file</param>
        /// <param name="checksumField">Checksum information</param>
        protected void CheckFile (string local_file, string signature, checksum checksumField)
        {
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
         
        /// <summary>Extract file</summary>
        /// <param name="dlobject">A <see cref="Download" /> instance</param>   
        protected void ExtractFile (object local_file_obj)
        {   
            string local_file = (string)local_file_obj;

            Log.Info("Decompressing {0} to {1}", local_file, this.InstallPath);            
            
            Decompresser decomp = new Decompresser(local_file, this.InstallPath);
            decomp.Run();
         
            if (decomp.DeleteFile)
                Log.Info("Deleting archive {0}", local_file);
                File.Delete(local_file);
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
        
        /// <summary>Check whether repository's XML file has to be downloaded again</summary>
        /// <exception cref="Nsm.RepositoryConnectionException">
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
                throw new RepositoryConnectionException("Connection to repository "+this.RepositoryURL+" failed", e);
            }
        }
            
        private void OnDownloadStatus (int dlid, double progress, int speed)
        {
            if (dlid == this.repo_dlid)
            {
                this.OnStatus (ActionType.UpdatingRepo, "", progress, speed);
            } else {
                this.OnStatus (ActionType.DownloadingPlugin, this.DownloadToPluginId[dlid], progress, speed);
            }
        }
        
        private void OnDownloadFinished (int dlid)
        {
            if (dlid == this.repo_dlid) {
                this.LoadRepository();
                return;
            }
                
            string plugin_id = this.DownloadToPluginId[dlid];
            string local_file = Path.Combine( this.InstallPath, Path.GetFileName(this.RepoItems[plugin_id].Url) );
        
            // Check file
            this.CheckFile(local_file, this.RepoItems[plugin_id].Signature, this.RepoItems[plugin_id].Checksum);
            
            // Extract archive
            Thread extractThread = new Thread( new ParameterizedThreadStart( this.ExtractFile ) );
            extractThread.Start( local_file );            
            while (extractThread.IsAlive)
            {
                this.OnStatus( ActionType.ExtractingPlugin, plugin_id, -1.0, -1);
                Thread.Sleep(SLEEP_TIME);
            }
            
            Log.Info("Updated plugin with id '{0}'", plugin_id);

            this.OnInstallFinished(plugin_id);
            this.DownloadToPluginId.Remove(dlid);
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
