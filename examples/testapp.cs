using System;
using System.Collections.Generic;
using System.Threading;
using NDesk.DBus;

public class TestCapuchin
{
	protected const string CAPUCHIN_SERVICE = "org.gnome.Capuchin";
	protected const string APPOBJECTMANAGER_PATH = "/org/gnome/Capuchin/AppObjectManager";
	
	protected Capuchin.IAppObjectManager manager;
	protected Capuchin.IAppObject appobject;
	
	private static TestCapuchin test = new TestCapuchin();
	
	public TestCapuchin()
	{
		Bus bus = Bus.Session;
		
		this.manager = bus.GetObject<Capuchin.IAppObjectManager> (CAPUCHIN_SERVICE, new ObjectPath (APPOBJECTMANAGER_PATH)); 
	}
	
	public void testGetAppObject()
	{
		ObjectPath path = this.manager.GetAppObject ("http://www.k-d-w.org/clipboard/deskbar/deskbar.xml");
		
		Bus bus = Bus.Session;
		
		this.appobject = bus.GetObject<Capuchin.IAppObject> (CAPUCHIN_SERVICE, path);
		this.appobject.InstallFinished += new Capuchin.InstallFinishedHandler ( this.OnInstallFinished );
		this.appobject.Status += new Capuchin.StatusHandler ( this.OnStatus );
		this.appobject.UpdateFinished += new Capuchin.UpdateFinishedHandler ( delegate () { ContinueMain(); } );
	}
	
	protected void OnInstallFinished(string plugin_id)
	{
		Console.WriteLine ("appobject updated: {0}", plugin_id);
	}
	
	protected void OnStatus(Capuchin.ActionType action, string plugin_id, double progress, int speed)
	{
		Console.WriteLine ("DOWNLOAD: {0} {1} {2} {3}", action, plugin_id, progress, speed);
	}
	
	public void testUpdate()
	{
		this.appobject.Update(false);
	}
	
	public void testGetAvailablePlugins()
	{
		string[] stuff = this.appobject.GetAvailablePlugins();
		Console.WriteLine ("ALL:");
		foreach (string s in stuff)
		{
			Console.WriteLine ("ID: " + s);
            Console.WriteLine ("Name: " + this.appobject.GetName(s));
            Console.WriteLine ("Description: " + this.appobject.GetDescription(s) );
            string[] author = this.appobject.GetAuthor(s);
            Console.WriteLine ("Author: {0} <{1}>", author[0], author[1]);
		}
	}
	
	public void testGetAvailableUpdates()
	{
		string[][] plugins = new string[2][];
		plugins[0] = new string[] {"leoorg.py", "0.2.0"};
		plugins[1] = new string[] {"ssh.py", "0.0.9"};
		
		string[] updates = this.appobject.GetAvailableUpdates (plugins);
		Console.WriteLine ("UPDATES:");
		foreach (string s in updates)
		{
			Console.WriteLine (s);
            Console.WriteLine ("Changes: " + this.appobject.GetChanges(s, "1.1.0.0"));
		}
	}
	
	public void testGetTags()
	{
		string[] tags = this.appobject.GetTags("leoorg.py");
		Console.WriteLine ("TAGS:");
		foreach (string t in tags)
		{
			Console.WriteLine (t);
		}
	}
	
	public void testGetAuthor()
	{
		string[] author = this.appobject.GetAuthor("leoorg.py");
		Console.WriteLine ("AUTHOR: {0}, {1}", author[0], author[1]);
	}
    
    public void testGetApplicationName () {
        Console.WriteLine ("APPLICATION-NAME: {0}", this.appobject.GetApplicationName ());
    }
	
	public void testInstall()
	{
		this.appobject.Install("leoorg.py");
	}
	
	public void testClose()
	{
		this.appobject.Close();
	}
	
	public static void ContinueMain()
	{
        test.testGetApplicationName();
        test.testGetAvailablePlugins();
		test.testGetAvailableUpdates();
		test.testGetTags();
		test.testGetAuthor();
		test.testInstall();
		Thread.Sleep(5000); // Wait 5s for update to complete, because we have no mainloop
		test.testClose();
	}
	
	public static void Main (string[] args)
	{
		test.testGetAppObject();
        test.testUpdate();
		Thread.Sleep(5000); // Wait 5s for update to complete, because we have no mainloop
	}
}
