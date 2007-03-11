#!/usr/bin/env python

import gobject
import dbus
if getattr(dbus, 'version', (0,0,0)) >= (0,41,0):
	import dbus.glib


def global_error_handler(e):
	print 'DBUS ERROR:', e
	

NEW_STUFF_SERVICE = 'org.gnome.NewStuffManager'
NEW_STUFF_IFACE = 'org.gnome.NewStuffManager.NewStuff'
NEW_STUFF_MANAGER_IFACE = 'org.gnome.NewStuffManager'
NEW_STUFF_MANAGER_PATH = '/org/gnome/NewStuffManager'
	
class TestNSM:
    
    def __init__(self):
        bus = dbus.SessionBus()
        proxy_obj_manager = bus.get_object(NEW_STUFF_SERVICE, NEW_STUFF_MANAGER_PATH)
        self.stuffmanager = dbus.Interface(proxy_obj_manager, NEW_STUFF_MANAGER_IFACE)
        
    def testGetNewStuff(self):
        path = self.stuffmanager.GetNewStuff('deskbarapplet')
        
        bus = dbus.SessionBus()
        proxy_obj_stuff = bus.get_object(NEW_STUFF_SERVICE, path)
        self.newstuff = dbus.Interface(proxy_obj_stuff, NEW_STUFF_IFACE)
        self.newstuff.connect_to_signal('Updated', self.__on_newstuff_updated)
        self.newstuff.connect_to_signal('DownloadStatus', self.__on_download_status)
        
    def testRefresh(self):
        self.newstuff.Refresh()
        
    def __del__(self):
        self.newstuff.Close()
        #self.bus.close()
        mainloop.quit()
    
    def __on_newstuff_updated(self, plugin_id):
        print "NewStuff updated:", plugin_id
	
    def __on_download_status(self, action, progress):
        print "DOWNLOAD:", action, progress
        
    def testGetAvailableNewStuff(self):
        def reply_GetAvailableNewStuff(newstuff):
            print "ALL:", newstuff
        self.newstuff.GetAvailableNewStuff(reply_handler=reply_GetAvailableNewStuff, error_handler=global_error_handler)
    
    def testGetAvailableUpdates(self):
        def reply_GetAvailableUpdates(updates):
            print "UPDATES:", updates
        plugins = [('leoorg.py', "0.2.0"),('ssh.py', "0.0.9")]
        self.newstuff.GetAvailableUpdates(plugins, reply_handler=reply_GetAvailableUpdates, error_handler=global_error_handler)
        
    def testGetTags(self):
        tags = self.newstuff.GetTags('ssh.py')
        print "TAGS:", tags
        tags = self.newstuff.GetTags('leoorg.py')
        print "TAGS:", tags
        
    def testGetAuthor(self):
    	author = self.newstuff.GetAuthor('leoorg.py')
    	print "AUTHOR:", author
        
    def testUpdate(self):
        self.newstuff.Update('leoorg.py')

if __name__=='__main__':
	gobject.threads_init()
	dbus.glib.init_threads()
	mainloop = gobject.MainLoop()
	test = TestNSM()
	test.testGetNewStuff()
	test.testRefresh()
	test.testGetAvailableNewStuff()
	test.testGetAvailableUpdates()
	test.testGetTags()
	test.testGetAuthor()
	test.testUpdate()
	del test
	mainloop.run()
