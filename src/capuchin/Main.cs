// project created on 17.12.2006 at 13:07
using System;
using System.Threading;
using Mono.GetOptions;
using NDesk.DBus;
using org.freedesktop.DBus;

class MainClass
{
    public const string OBJECT_SERVICE = "org.gnome.Capuchin";
    public const string CAPUCHIN_OBJECT_PATH = "/org/gnome/Capuchin";
    public const string DOWNLOADMANAGER_OBJECT_PATH = "/org/gnome/Capuchin/DownloadManager";
	
	static Options opts = new Options();

	public static void Main(string[] args)
	{
		opts.ProcessArgs(args);
		
		Capuchin.Capuchin nsm = new Capuchin.Capuchin();
		Bus.Session.Register(OBJECT_SERVICE, new ObjectPath(CAPUCHIN_OBJECT_PATH), nsm);
		
		Capuchin.DownloadManager dlm = new Capuchin.DownloadManager();
		Bus.Session.Register(OBJECT_SERVICE, new ObjectPath(DOWNLOADMANAGER_OBJECT_PATH), dlm);
		
		RequestNameReply reply = Bus.Session.RequestName(OBJECT_SERVICE);
		Console.WriteLine("RequestName: "+reply);
		
		try {
		while (true)
			Bus.Session.Iterate();
		} catch (Exception e) {
			Console.WriteLine(e);
		}
	}
}
