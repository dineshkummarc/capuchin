// project created on 17.12.2006 at 13:07
using System;
using System.Threading;
using System.IO;
using Mono.GetOptions;
using NDesk.DBus;
using org.freedesktop.DBus;

class CapuchinOptions : Options
{
    public CapuchinOptions ()
    {
        base.ParsingMode = OptionsParsingMode.Both;
    }   

    [Option("Enable debug mode", 'd', "debug")]
    public bool debug;
}

class MainClass
{
    public const string OBJECT_SERVICE = "org.gnome.Capuchin";
    public const string APPOBJECTMANAGER_OBJECT_PATH = "/org/gnome/Capuchin/AppObjectManager";
    public const string DOWNLOADMANAGER_OBJECT_PATH = "/org/gnome/Capuchin/DownloadManager";
	
	static CapuchinOptions opts = new CapuchinOptions();

	public static void Main(string[] args)
	{
		opts.ProcessArgs(args);
	    CreateLogDirIfNotExists();
		
        if (opts.debug)
        {
            Capuchin.Logging.Log.Initialize(Capuchin.Globals.Instance.LOG_FILES_DIR,
                                            "capuchin",
                                            Capuchin.Logging.LogLevel.Debug,
                                            true
                                            );
        } else {
            Capuchin.Logging.Log.Initialize(Capuchin.Globals.Instance.LOG_FILES_DIR,
                                            "capuchin",
                                            Capuchin.Logging.LogLevel.Error,
                                            true
                                            );
        }
        
		Capuchin.AppObjectManager obj_manager = new Capuchin.AppObjectManager();
		Bus.Session.Register(OBJECT_SERVICE, new ObjectPath(APPOBJECTMANAGER_OBJECT_PATH), obj_manager);
		
		Capuchin.DownloadManager dlm = new Capuchin.DownloadManager();
		Bus.Session.Register(OBJECT_SERVICE, new ObjectPath(DOWNLOADMANAGER_OBJECT_PATH), dlm);
		
		RequestNameReply reply = Bus.Session.RequestName(OBJECT_SERVICE);
		Capuchin.Logging.Log.Debug("RequestName: "+reply);
		
		try {
		while (true)
			Bus.Session.Iterate();
		} catch (Exception e) {
			Console.WriteLine(e);
		}
	}
	
    private static void CreateLogDirIfNotExists()
	{
	   if (!Directory.Exists(Capuchin.Globals.Instance.LOG_FILES_DIR))
       {
          Capuchin.Logging.Log.Debug("Creating directory {0}", Capuchin.Globals.Instance.LOG_FILES_DIR);
           Directory.CreateDirectory(Capuchin.Globals.Instance.LOG_FILES_DIR);
       }
    }

}
