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
			this.printPluginInfos (s);
		}
	}
	
	private void printPluginInfos (string s) {
		Console.WriteLine ("ID: " + s);
        Console.WriteLine ("Name: " + this.appobject.GetPluginName(s));
        Console.WriteLine ("Description: " + this.appobject.GetPluginDescription(s) );
        string[] author = this.appobject.GetPluginAuthor(s);
        Console.WriteLine ("Author: {0} <{1}>", author[0], author[1]);
		string[] tags = this.appobject.GetPluginTags(s);
		Console.WriteLine ("TAGS for {0}:", s);
		foreach (string t in tags)
		{
			Console.WriteLine (t);
		}
		Console.WriteLine();
	}
	
	public void testGetAvailableUpdates()
	{
		string[][] plugins = new string[2][];
		plugins[0] = new string[] {"ssh.py", "0.0.9"};
		plugins[1] = new string[] {"ekiga.py", "0.1.0"};
		
		string[] updates = this.appobject.GetAvailableUpdates (plugins);
		Console.WriteLine ("UPDATES:");
		foreach (string s in updates)
		{
			Console.WriteLine (s);
            Console.WriteLine ("Changes: " + this.appobject.GetPluginChanges(s, "1.1.0.0"));
		}
	}
	
    public void testGetApplicationName () {
        Console.WriteLine ("APPLICATION-NAME: {0}", this.appobject.GetApplicationName ());
    }
	
	public void testGetTags () {
		string[] tags = this.appobject.GetTags ();
		Console.WriteLine ("TAGS:");
		foreach (string t in tags)
		{
			Console.WriteLine (t);
		}
	}
	
	public void testGetPluginsWithTag () {
		Console.WriteLine("PLUGINS WITH TAG:");
		string[] ids = this.appobject.GetPluginsWithTag ("phone");
		foreach (string id in ids) {
			this.printPluginInfos (id);
		}
	}
	
	public void testInstall()
	{
		this.appobject.Install("ekiga.py");
	}
	
	public void testClose()
	{
		this.appobject.Close();
	}
	
	public static void Main (string[] args)
	{
		test.testGetAppObject();
        test.testUpdate();
		test.testGetApplicationName();
        test.testGetAvailablePlugins();
		test.testGetAvailableUpdates();
		test.testGetTags ();
		test.testGetPluginsWithTag ();
		test.testInstall();
		Thread.Sleep(10000); // Wait 5s for update to complete, because we have no mainloop
		test.testClose();
	}
}
