#!/usr/bin/env python

import gobject
import dbus, dbus.glib

REPO_URL = "http://www.k-d-w.org/clipboard/deskbar/deskbar.xml"

CAPUCHIN_SERVICE = "org.gnome.Capuchin"
APPOBJECTMANAGER_PATH = "/org/gnome/Capuchin/AppObjectManager"
APPOBJECTMANAGER_IFACE = "org.gnome.Capuchin"
APPOBJECT_IFACE = "org.gnome.Capuchin.AppObject"

# We define some plugins here
PLUGINS = [('ekiga.py', '0.1.0'), # id, version
           ('yahoo.py', '0.3.0'),
           ('gmail-deskbar-hack.py', '0.99.9.99'),
          ]

class TestApp:
    
    def __init__(self):
        bus = dbus.SessionBus()
        # Get proxy object
        proxy_obj_manager = bus.get_object(CAPUCHIN_SERVICE, APPOBJECTMANAGER_PATH)
        # Apply the correct interace to the proxy object
        self.appobjectmanager = dbus.Interface(proxy_obj_manager, APPOBJECTMANAGER_IFACE)
        
        object_path = self.appobjectmanager.GetAppObject(REPO_URL)
        
        proxy_obj = bus.get_object(CAPUCHIN_SERVICE, object_path)
        self.appobject = dbus.Interface(proxy_obj, APPOBJECT_IFACE)
        
        self.appobject.connect_to_signal('InstallFinished', self.on_install_finished)
        self.appobject.connect_to_signal('Status', self.on_status)
        
    def on_install_finished(self, plugin_id):
        print "appobject updated: %s" % plugin_id
        
    def on_update_finished(self):
        self.testGetApplicationName();
        self.testGetAvailablePlugins();
        self.testGetAvailableUpdates();
        self.testGetTags ();
        self.testGetPluginsWithTag ();
        self.testInstall ();
        
    def on_install_finished(self, plugin_id):
        print "appobject updated: %s" % plugin_id
        self.appobject.Close() # We did everything we wanted to
	
    def on_status(self, action, plugin_id, progress, speed):
        print "DOWNLOAD: %s %s %s %s" % (action, plugin_id, progress, speed)
        
    def on_error(self, error):
        print "ERROR: " +error
        
    def testUpdate(self):
        self.appobject.Update(False, reply_handler=self.on_update_finished, error_handler=self.on_error)
        
    def testGetAvailablePlugins(self):
        stuff = self.appobject.GetAvailablePlugins();
        print "ALL:"
        for s in stuff:
            self.printPluginInfos (s)
			
    def printPluginInfos(self, s):
        print "ID: " + s
        print "Name: " + self.appobject.GetPluginName(s)
        print "Description: " + self.appobject.GetPluginDescription(s)
        author = self.appobject.GetPluginAuthor(s)
        print "Author: %s <%s>" % (author[0], author[1])
        tags = self.appobject.GetPluginTags(s);
        print "TAGS for %s:" % s
        for t in tags:
	        print t
        print
        
    def testGetApplicationName(self):
        print "APPLICATION-NAME: "+ self.appobject.GetApplicationName ()
        
    def testGetAvailableUpdates(self):
        updates = self.appobject.GetAvailableUpdates (PLUGINS)
        print "UPDATES:"
        for s in updates:
            print s
            print "Changes: "+self.appobject.GetPluginChanges(s, "1.1.0.0")
            
    def testGetTags(self):
        tags = self.appobject.GetTags ()
        print "TAGS:"
        for t in tags:
            print t
            
    def testGetPluginsWithTag(self):
        print "PLUGINS WITH TAG:"
        ids = self.appobject.GetPluginsWithTag ("phone")
        for i in ids:
            print self.printPluginInfos (i)
            
    def testInstall(self):
        self.appobject.Install("ekiga.py")
    
if __name__=='__main__':
    gobject.threads_init()
    dbus.glib.init_threads()
    mainloop = gobject.MainLoop()
    app = TestApp()
    app.testUpdate ()
    
    mainloop.run()
