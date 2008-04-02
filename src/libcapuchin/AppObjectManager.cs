/*
 * This file is part of Capuchin
 * 
 * Copyright (C) Sebastian Pölsterl 2008 <marduk@k-d-w.org>
 * 
 * Capuchin is free software.
 * 
 * You may redistribute it and/or modify it under the terms of the
 * GNU General Public License, as published by the Free Software
 * Foundation; either version 2 of the License, or (at your option)
 * any later version.
 * 
 * Capuchin is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Capuchin.  If not, write to:
 * 	The Free Software Foundation, Inc.,
 * 	51 Franklin Street, Fifth Floor
 * 	Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using NDesk.DBus;
using org.freedesktop.DBus;
using Capuchin.Logging;
using System.Text.RegularExpressions;

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
            
			string object_path = String.Format(CAPUCHIN_PATH,
			                                   this.MakeObjectPath(this.GetApplicationName(repository_url)) );
            ObjectPath new_stuff_opath = new ObjectPath (object_path);
 
            Log.Info ("Creating app object for {0} at {1}", repository_url, object_path);
            AppObject nsm = new AppObject(repository_url);
            nsm.Closed += new AppObject.ClosedHandler( this.OnClosed );
            Bus.Session.Register(CAPUCHIN_SERVICE, new_stuff_opath, nsm);
            
            this.Objects.Add(repository_url, new_stuff_opath);
            
            return new_stuff_opath;
        }
        
		protected string MakeObjectPath (string path)
		{
			Regex rx = new Regex (@"[^A-Za-z0-9_]");
				
			path = path.Replace ("/", "_");
			return rx.Replace (path, "_");
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
