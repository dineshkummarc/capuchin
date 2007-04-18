#!/usr/bin/env python

import gobject
import dbus, dbus.glib

NEW_STUFF_SERVICE = 'org.gnome.NewStuffManager'
NEW_STUFF_IFACE = 'org.gnome.NewStuffManager.NewStuff'
NEW_STUFF_MANAGER_IFACE = 'org.gnome.NewStuffManager'
NEW_STUFF_MANAGER_PATH = '/org/gnome/NewStuffManager'

# We define some plugins here
PLUGINS = [('leoorg.py', '0.1.0'), # id, version
           ('yahoo.py', '0.3.0'),
           ('gmail-deskbar-hack.py', '0.99.9.99'),
          ]

def global_error_handler(e):
    print "DBUS ERROR:", e

class TestApp:
    
    def __init__(self):
        bus = dbus.SessionBus()
        # Get proxy object of NewStuffManager
        proxy_obj_manager = bus.get_object(NEW_STUFF_SERVICE, NEW_STUFF_MANAGER_PATH)
        # Apply the correct interace to the proxy object
        self.newstuffmanager = dbus.Interface(proxy_obj_manager, NEW_STUFF_MANAGER_IFACE)
        
        object_path = self.newstuffmanager.GetNewStuff('deskbarapplet') # deskbarapplet = <your_application_name>
        
        proxy_obj_stuff = bus.get_object(NEW_STUFF_SERVICE, object_path)
        self.newstuff = dbus.Interface(proxy_obj_stuff, NEW_STUFF_IFACE)
        
        self.newstuff.connect_to_signal('Updated', self.on_newstuff_updated)
        self.newstuff.connect_to_signal('DownloadStatus', self.on_download_status)
        
    def on_newstuff_updated(self, plugin_id):
        print "NewStuff updated:", plugin_id
	
    def on_download_status(self, action, progress):
        # Progress is either between 0.0 and 1.0 while downloading
        # or -1.0 while extracting to point out that the exact progress
        # is unknown. A gtk.ProgressBar should pulse here.
        print "Current action: %s, progress: %s" % (action, progress)
        
    def load_repo(self):
        self.newstuff.Refresh()
        
    def get_all(self):
        def reply_GetAvailableNewStuff(newstuff):
            print "The following stuff is available:"
            for plugin_id, name, desc in newstuff:
                print "ID: %s, NAME: %s, DESCRIPTION: %s" % (plugin_id, name, desc)
            print
        self.newstuff.GetAvailableNewStuff(reply_handler=reply_GetAvailableNewStuff, error_handler=global_error_handler)
        
    def get_updates(self):
         def reply_GetAvailableUpdates(updates):
            available_updates = []
            print "The following updates are available:"
            for plugin_id, desc in updates:
                available_updates.append(plugin_id)
                print "ID: %s, DESC: %s" % (plugin_id, desc)
            print
            self.update_plugins(available_updates)
         self.newstuff.GetAvailableUpdates(PLUGINS, reply_handler=reply_GetAvailableUpdates, error_handler=global_error_handler)
        
    def update_plugins(self, available_updates):
        for plugin_id in available_updates:
            print "Starting update for %s" % plugin_id
            self.newstuff.Update(plugin_id)
            
    def finalize(self):
        self.newstuff.Close()
        
if __name__=='__main__':
    gobject.threads_init()
    dbus.glib.init_threads()
    mainloop = gobject.MainLoop()
    app = TestApp()
    app.load_repo()
    app.get_all()
    app.get_updates()
    app.finalize()
    mainloop.run()
