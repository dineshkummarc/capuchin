// project created on 17.12.2006 at 13:08
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using NDesk.DBus;
using org.freedesktop.DBus;

namespace Nsm
{
    /// <summary>Entry point for applications to request their own <see cref="Nsm.NewStuff /> object</summary>
	[Interface("org.gnome.NewStuffManager")]
	public class NewStuffManager : MarshalByRefObject
	{
		
		public const string NEW_STUFF_SERVICE = "org.gnome.NewStuffManager";
	    public const string NEW_STUFF_PATH = "/org/gnome/NewStuffManager/{0}";
	    
	    protected Dictionary<string, ObjectPath> Objects;
	    
	    public NewStuffManager()
	    {
	        this.Objects = new Dictionary<string, ObjectPath>();
	        this.CreateCacheIfNotExists();
	        Gnome.Vfs.Vfs.Initialize ();
	    }
	    
	    /// <summary>Request an application specific <see cref="Nsm.NewStuff /> object</summary>
	    /// <param name="application_name">Name of the application</param>
	    /// <remarks><code>application_name</code> must be the name of a configuration file without the .conf ending</remarks>
    	public virtual ObjectPath GetNewStuff(string application_name) {
    	    if (this.Objects.ContainsKey(application_name))
    	        return this.Objects[application_name];
    	    
    		ObjectPath new_stuff_opath = new ObjectPath (String.Format(NEW_STUFF_PATH, application_name));
 
            NewStuff nsm = new NewStuff(application_name);
    	    nsm.Closed += new NewStuff.ClosedHandler( this.OnClosed );
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
