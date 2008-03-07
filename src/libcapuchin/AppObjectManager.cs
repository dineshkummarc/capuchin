// project created on 17.12.2006 at 13:08
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using NDesk.DBus;
using org.freedesktop.DBus;
using Capuchin.Logging;

namespace Capuchin
{
    
    public class AppObjectManager : IAppObjectManager
    {

        public const string CAPUCHIN_SERVICE = "org.gnome.Capuchin";
        public const string CAPUCHIN_PATH = "/org/gnome/Capuchin/{0}";
        
        protected Dictionary<string, ObjectPath> Objects;
        
        public AppObjectManager()
        {
            this.Objects = new Dictionary<string, ObjectPath>();
            this.CreateCacheIfNotExists();
            Gnome.Vfs.Vfs.Initialize ();
        }
        
        /// <summary>Request an application specific <see cref="Capuchin.AppObject /> object</summary>
        /// <param name="repository_url">URL to repository's XML file</param>
        public virtual ObjectPath GetAppObject(string repository_url) {
            if (this.Objects.ContainsKey(repository_url))
                return this.Objects[repository_url];
            
            string object_path = String.Format(CAPUCHIN_PATH, this.GetApplicationName(repository_url));
            ObjectPath new_stuff_opath = new ObjectPath (object_path);
 
            Log.Info ("Creating app object for {0} at {1}", repository_url, object_path);
            AppObject nsm = new AppObject(repository_url);
            nsm.Closed += new AppObject.ClosedHandler( this.OnClosed );
            Bus.Session.Register(CAPUCHIN_SERVICE, new_stuff_opath, nsm);
            
            this.Objects.Add(repository_url, new_stuff_opath);
            
            return new_stuff_opath;
        }
        
        protected void OnClosed(string repository_url)
        {
            Log.Info ("Unregistering {0}", this.Objects[repository_url]);          
            
            Bus.Session.Unregister( CAPUCHIN_SERVICE, this.Objects[repository_url]);
            this.Objects.Remove(repository_url);
        }
        
        private void CreateCacheIfNotExists()
        {
           if (!Directory.Exists(Globals.Instance.LOCAL_CACHE_DIR))
           {
               Log.Info("Creating directory {0}", Globals.Instance.LOCAL_CACHE_DIR);
               Directory.CreateDirectory(Globals.Instance.LOCAL_CACHE_DIR);
           }
        }

        private string GetApplicationName(string repository_url)
        {
            return Path.GetFileName(repository_url).Split('.')[0];
        }
    }
}
