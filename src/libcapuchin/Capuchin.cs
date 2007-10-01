// project created on 17.12.2006 at 13:08
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using NDesk.DBus;
using org.freedesktop.DBus;

namespace Capuchin
{
    /// <summary>Entry point for applications to request their own <see cref="Nsm.NewStuff /> object</summary>
	[Interface("org.gnome.Capuchin")]
	public interface ICapuchin
	{
		ObjectPath GetAppObject(string app_name);
	}
	
	public class Capuchin : ICapuchin
	{

        public const string NEW_STUFF_SERVICE = "org.gnome.Capuchin";
        public const string NEW_STUFF_PATH = "/org/gnome/Capuchin/{0}";
	    
	    protected Dictionary<string, ObjectPath> Objects;
	    
	    public Capuchin()
	    {
	        this.Objects = new Dictionary<string, ObjectPath>();
	        this.CreateCacheIfNotExists();
	        Gnome.Vfs.Vfs.Initialize ();
	    }
	    
	    /// <summary>Request an application specific <see cref="Nsm.NewStuff /> object</summary>
	    /// <param name="application_name">Name of the application</param>
	    /// <remarks><code>application_name</code> must be the name of a configuration file without the .conf ending</remarks>
    	public virtual ObjectPath GetAppObject(string application_name) {
    	    if (this.Objects.ContainsKey(application_name))
    	        return this.Objects[application_name];
    	    
    		ObjectPath new_stuff_opath = new ObjectPath (String.Format(NEW_STUFF_PATH, application_name));
 
            AppObject nsm = new AppObject(application_name);
    	    nsm.Closed += new AppObject.ClosedHandler( this.OnClosed );
    	    Bus.Session.Register(NEW_STUFF_SERVICE, new_stuff_opath, nsm);
    	    
    	    this.Objects.Add(application_name, new_stuff_opath);
			
			return new_stuff_opath;
    	}
    	
    	protected void OnClosed(string application_name)
    	{
    	    Bus.Session.Unregister( NEW_STUFF_SERVICE, this.Objects[application_name]);
    	    this.Objects.Remove(application_name);
    	}
    	
    	private void CreateCacheIfNotExists()
    	{
    	   if (!Directory.Exists(Globals.Instance.LOCAL_CACHE_DIR))
	       {
	           Directory.CreateDirectory(Globals.Instance.LOCAL_CACHE_DIR);
	       }
	    }
	}
}
