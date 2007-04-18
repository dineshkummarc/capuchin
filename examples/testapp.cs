using System;
using System.Collections.Generic;
using System.Threading;
using NDesk.DBus;

[Interface("org.gnome.NewStuffManager")]
public interface INewStuffManager
{
	ObjectPath GetNewStuff(string application_name);
}

public delegate void UpdatedHandler(string plugin_id);
public delegate void DownloadStatusHandler(string action, double progress);
[Interface("org.gnome.NewStuffManager.NewStuff")]
public interface INewStuff
{
	event UpdatedHandler Updated;
	event DownloadStatusHandler DownloadStatus;
	
	void Update(string plugin_id);    	
	void Refresh();
	string[][] GetAvailableNewStuff();
	string[][] GetAvailableUpdates(string[][] plugins);
	string[] GetTags(string plugin_id);
	IDictionary<string, string> GetAuthor(string plugin_id);
	void Close();
}

public class TestNSM
{
	protected const string NEW_STUFF_SERVICE = "org.gnome.NewStuffManager";
	protected const string NEW_STUFF_MANAGER_PATH = "/org/gnome/NewStuffManager";
	
	protected INewStuffManager stuffmanager;
	protected INewStuff newstuff;
	
	public TestNSM()
	{
		Bus bus = Bus.Session;
		
		this.stuffmanager = bus.GetObject<INewStuffManager> (NEW_STUFF_SERVICE, new ObjectPath (NEW_STUFF_MANAGER_PATH)); 
	}
	
	public void testGetNewStuff()
	{
		ObjectPath path = this.stuffmanager.GetNewStuff ("deskbarapplet");
		
		Bus bus = Bus.Session;
		
		this.newstuff = bus.GetObject<INewStuff> (NEW_STUFF_SERVICE, path);
		this.newstuff.Updated += new UpdatedHandler ( this.OnNewStuffUpdated );
		this.newstuff.DownloadStatus += new DownloadStatusHandler ( this.OnDownloadStatus );
	}
	
	protected void OnNewStuffUpdated(string plugin_id)
	{
		Console.WriteLine ("NewStuff updated: {0}", plugin_id);
	}
	
	protected void OnDownloadStatus(string action, double progress)
	{
		Console.WriteLine ("DOWNLOAD: {0} {1}", action, progress);
	}
	
	public void testRefresh()
	{
		this.newstuff.Refresh();
	}
	
	public void testGetAvailableNewStuff()
	{
		string[][] stuff = this.newstuff.GetAvailableNewStuff();
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
