using System;
using System.Collections.Generic;
using System.Threading;
using NDesk.DBus;

public class TestNSM
{
	protected const string CAPUCHIN_SERVICE = "org.gnome.Capuchin";
	protected const string CAPUCHIN_PATH = "/org/gnome/Capuchin";
	
	protected Capuchin.ICapuchin stuffmanager;
	protected Capuchin.IAppObject newstuff;
	
	public TestNSM()
	{
		Bus bus = Bus.Session;
		
		this.stuffmanager = bus.GetObject<Capuchin.ICapuchin> (CAPUCHIN_SERVICE, new ObjectPath (CAPUCHIN_PATH)); 
	}
	
	public void testGetNewStuff()
	{
		ObjectPath path = this.stuffmanager.GetAppObject ("http://www.k-d-w.org/clipboard/deskbar/deskbar.xml");
		
		Bus bus = Bus.Session;
		
		this.newstuff = bus.GetObject<Capuchin.IAppObject> (CAPUCHIN_SERVICE, path);
		this.newstuff.Updated += new Capuchin.UpdatedHandler ( this.OnNewStuffUpdated );
		this.newstuff.InstallationStatus += new Capuchin.InstallationStatusHandler ( this.OnDownloadStatus );
	}
	
	protected void OnNewStuffUpdated(string plugin_id)
	{
		Console.WriteLine ("NewStuff updated: {0}", plugin_id);
	}
	
	protected void OnDownloadStatus(string plugin_id, string action, double progress, int speed)
	{
		Console.WriteLine ("DOWNLOAD: {0} {1} {2} {3}", plugin_id, action, progress, speed);
	}
	
	public void testRefresh()
	{
		this.newstuff.Refresh(false);
	}
	
	public void testGetAvailableNewStuff()
	{
		string[][] stuff = this.newstuff.GetAvailablePlugins();
		Console.WriteLine ("ALL:");
		foreach (string[] s in stuff)
		{
			Console.WriteLine ("{0}, {1}, {2}", s[0], s[1], s[2]);
		}
	}
	
	public void testGetAvailableUpdates()
	{
		string[][] plugins = new string[2][];
		plugins[0] = new string[] {"leoorg.py", "0.2.0"};
		plugins[1] = new string[] {"ssh.py", "0.0.9"};
		
		string[][] updates = this.newstuff.GetAvailableUpdates (plugins);
		Console.WriteLine ("UPDATES:");
		foreach (string[] s in updates)
		{
			Console.WriteLine ("{0}, {1}", s[0], s[1]);
		}
	}
	
	public void testGetTags()
	{
		string[] tags = this.newstuff.GetTags("leoorg.py");
		Console.WriteLine ("TAGS:");
		foreach (string t in tags)
		{
			Console.WriteLine (t);
		}
	}
	
	public void testGetAuthor()
	{
		IDictionary<string, string> author = this.newstuff.GetAuthor("leoorg.py");
		Console.WriteLine ("AUTHOR: {0}, {1}", author["name"], author["email"]);
	}
	
	public void testUpdate()
	{
		this.newstuff.Update("leoorg.py");
	}
	
	public void testClose()
	{
		this.newstuff.Close();
	}
	
	public static void Main (string[] args)
	{
		TestNSM test = new TestNSM();
		test.testGetNewStuff();
		test.testRefresh();
		test.testGetAvailableNewStuff();
		test.testGetAvailableUpdates();
		test.testGetTags();
		test.testGetAuthor();
		test.testUpdate();
		Thread.Sleep(5000); // Wait 5s for update to complete, because we have no mainloop
		test.testClose();
	}
}
