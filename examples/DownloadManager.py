#!/usr/bin/env python

import gobject
import dbus
import time
if getattr(dbus, 'version', (0,0,0)) >= (0,41,0):
	import dbus.glib


def global_error_handler(e):
	print 'DBUS ERROR:', e
	

NEW_STUFF_SERVICE = 'org.gnome.NewStuffManager'
DOWNLOAD_MANAGER_IFACE = 'org.gnome.NewStuffManager.DownloadManager'
DOWNLOAD_MANAGER_PATH = '/org/gnome/NewStuffManager/DownloadManager'
	
class TestNSM:
    
    def __init__(self):
        bus = dbus.SessionBus()
        proxy_obj_manager = bus.get_object(NEW_STUFF_SERVICE, DOWNLOAD_MANAGER_PATH)
        self.stuffmanager = dbus.Interface(proxy_obj_manager, DOWNLOAD_MANAGER_IFACE)
        self.stuffmanager.connect_to_signal('DownloadStatus', self.__on_download_status)
		
    def __on_download_status(self, dlid, action, progress):
        print "DOWNLOAD:", dlid, action, progress
		
    def testDownload(self):
    	downloads = [
    	"http://go-mono.com/sources/monodevelop/monodevelop-0.13.1.tar.gz",
    	"http://download.wengo.com/releases/WengoPhone-2.1/RC2/linux/WengoPhone-2.1-rc2-linux-bin-x86.tar.bz2",
    	#"http://www.ekiga.org/admin/downloads/latest/sources/sources/ekiga-2.0.9.tar.gz",
    	"http://download.gizmoproject.com/GizmoDownload/gizmo-project-2.0.0.56.tar.gz"
    	]
    	for dl in downloads:
	    	dlid = self.stuffmanager.DownloadFile(dl, "/home/sebp/Downloads");
    		print "DownloadID is %i" % dlid
	    	#time.sleep(2)
    		#print "Pausing"
    		#self.stuffmanager.PauseDownload(dlid)
    		#time.sleep(5)
    		#print "Abort"
	    	#self.stuffmanager.AbortDownload(dlid)
    		#print "Resume"
    		#self.stuffmanager.ResumeDownload(dlid)    	

if __name__=='__main__':
	gobject.threads_init()
	dbus.glib.init_threads()
	mainloop = gobject.MainLoop()
	test = TestNSM()
	test.testDownload()
	mainloop.run()
