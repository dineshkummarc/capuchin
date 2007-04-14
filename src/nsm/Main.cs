// project created on 17.12.2006 at 13:07
using System;
using System.Threading;
using Mono.GetOptions;
using NDesk.DBus;
using Nsm;
using org.freedesktop.DBus;

namespace newstuffmanager
{
	class MainClass
	{		
		public const string OBJECT_SERVICE = "org.gnome.NewStuffManager";
		public const string OBJECT_PATH = "/org/gnome/NewStuffManager";
		
		static Options opts = new Options();
	
		public static void Main(string[] args)
		{
			opts.ProcessArgs(args);
			
			NewStuffManager nsm = new NewStuffManager();
			Bus.Session.Register(OBJECT_SERVICE, new ObjectPath(OBJECT_PATH), nsm);
			
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
}