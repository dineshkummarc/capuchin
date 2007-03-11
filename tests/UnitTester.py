#!/usr/bin/env python

import unittest
import gobject
import dbus
if getattr(dbus, 'version', (0,0,0)) >= (0,41,0):
	import dbus.glib


def global_error_handler(e):
	print 'DBUS ERROR:', e

class TestNSM(unittest.TestCase):
    
    def setUp(self):
        self.bus = dbus.SessionBus()
        proxy_obj_manager = self.bus.get_object('org.gnome.NewStuffManager', '/org/gnome/NewStuffManager')
        self.stuffmanager = dbus.Interface(proxy_obj_manager, 'org.gnome.NewStuffManager')
        
        #def test1GetNewStuff(self):
        service, path = self.stuffmanager.GetNewStuff('deskbarapplet')
        
        proxy_obj_stuff = self.bus.get_object(service, path)
        self.newstuff = dbus.Interface(proxy_obj_stuff, 'org.gnome.NewStuffManager.NewStuff')
        self.newstuff.connect_to_signal('Updated', self.__on_newstuff_updated)
        self.newstuff.connect_to_signal('DownloadStatus', self.__on_download_status)
        #self.assert_(True)
        
    def testRefresh(self):
        self.newstuff.Refresh()
        self.assert_(True)
        
    def tearDown(self):
        self.newstuff.Close()
        self.bus.close()
        mainloop.quit()
    
    def __on_newstuff_updated(self, plugin_id):
        print "NewStuff updated:", plugin_id
    	self.newstuff.Close()
	
    def __on_download_status(self, action, progress):
        print "DOWNLOAD:", action, progress
        
    def testGetAvailableNewStuff(self):
        def reply_GetAvailableNewStuff(newstuff):
            print "ALL:", newstuff
            self.assert_(True)
        self.newstuff.GetAvailableNewStuff(reply_handler=reply_GetAvailableNewStuff, error_handler=global_error_handler)
    
    def testGetAvailableUpdates(self):
        def reply_GetAvailableUpdates(updates):
            print "UPDATES:", updates
            self.assertEquals(updates, [(u'leoorg.py', u'Query dict.leo.org')])
        plugins = [['leoorg.py','0.2.0'],['ssh.py','0.0.9']]
        self.newstuff.GetAvailableUpdates(plugins, reply_handler=reply_GetAvailableUpdates, error_handler=global_error_handler)
        
    def testGetTags(self):
        tags = self.newstuff.GetTags('ssh.py')
        print "TAGS:", tags
        self.assertEquals(tags, [])
        #tags = self.newstuff.GetTags('leoorg.py')
        #self.assertEqual(tags, )
        
    def testUpdate(self):
        self.newstuff.Update('leoorg.py')

if __name__=='__main__':
	gobject.threads_init()
	dbus.glib.init_threads()
	mainloop = gobject.MainLoop()
	suite = unittest.TestLoader().loadTestsFromTestCase(TestNSM)
	unittest.TextTestRunner(verbosity=2).run(suite)
	mainloop.run()
