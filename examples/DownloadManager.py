#!/usr/bin/env python

import gobject
import dbus
import gtk
if getattr(dbus, 'version', (0,0,0)) >= (0,41,0):
	import dbus.glib

def global_error_handler(e):
	print 'DBUS ERROR:', e	

NEW_STUFF_SERVICE = 'org.gnome.NewStuffManager'
DOWNLOAD_MANAGER_IFACE = 'org.gnome.NewStuffManager.DownloadManager'
DOWNLOAD_MANAGER_PATH = '/org/gnome/NewStuffManager/DownloadManager'

DOWNLOAD_DIR = '/home/sebp/Downloads'

class DownloadManagerWindow(gtk.Window):

	def __init__(self):
		gtk.Window.__init__(self)
		
		self.dlid_to_iter = {}
		
		self.set_title("Download Manager")
		self.set_default_size(500, 400)
		self.set_border_width(6)
		self.connect('delete-event', self.__on_quit)
		self.connect('destroy-event', self.__on_quit)
		self.vbox = gtk.VBox(spacing=12)
		self.add(self.vbox)
		
		self.toolbar = gtk.Toolbar()
		self.button_pause = gtk.ToolButton(gtk.STOCK_MEDIA_PAUSE)
		self.button_pause.connect("clicked", self.__on_pause)
		self.button_pause.set_sensitive(False)
		self.toolbar.insert(self.button_pause, 0)
		
		self.button_stop = gtk.ToolButton(gtk.STOCK_MEDIA_STOP)
		self.button_stop.connect("clicked", self.__on_stop)
		self.button_stop.set_sensitive(False)
		self.toolbar.insert(self.button_stop, 1)
		
		self.vbox.pack_start(self.toolbar, False, False, 0)
		
		self.scrolledwindow = gtk.ScrolledWindow()
		self.scrolledwindow.set_policy(gtk.POLICY_AUTOMATIC,gtk.POLICY_AUTOMATIC)
		self.scrolledwindow.set_shadow_type(gtk.SHADOW_ETCHED_IN)
		self.vbox.pack_start(self.scrolledwindow, True, True, 0)
		
		self.treeview = gtk.TreeView()
		self.treeview.connect("cursor-changed", self.__on_cursor_changed)
		
		self.cell_progress = gtk.CellRendererProgress()
		col_progress = gtk.TreeViewColumn('Progress', self.cell_progress, text=3, value=2)
		col_progress.set_resizable(True)
		self.treeview.append_column(col_progress)
		
		cell_id = gtk.CellRendererText()
		col_id = gtk.TreeViewColumn('ID', cell_id, text=0)
		col_id.set_resizable(True)
		self.treeview.append_column(col_id)
		
		cell_url = gtk.CellRendererText()
		col_url = gtk.TreeViewColumn('URL', cell_url, text=1)
		col_url.set_resizable(True)
		self.treeview.append_column(col_url)
		
		self.scrolledwindow.add(self.treeview)
		
		self.liststore = gtk.ListStore(int, str, float, str, bool) # ID, url, progress, progress_str, paused
		self.treeview.set_model(self.liststore)
		
		self.hbox = gtk.HBox(spacing=6)
		self.vbox.pack_start(self.hbox, False)
		
		self.url_entry = gtk.Entry()
		self.hbox.pack_start(self.url_entry, True, True, 0)
		
		self.button = gtk.Button(stock=gtk.STOCK_ADD)
		self.button.connect('clicked', self.__on_button_clicked)
		self.hbox.pack_start(self.button, False)
		
		# Setup D-Bus
		bus = dbus.SessionBus()
		proxy_obj_manager = bus.get_object(NEW_STUFF_SERVICE, DOWNLOAD_MANAGER_PATH)
		self.downloadmanager = dbus.Interface(proxy_obj_manager, DOWNLOAD_MANAGER_IFACE)
		self.downloadmanager.connect_to_signal('DownloadStatus', self.__on_download_status)
		
	def __on_button_clicked(self, button):
		url = self.url_entry.get_text()
		dlid = self.downloadmanager.DownloadFile(url, DOWNLOAD_DIR)
		self.dlid_to_iter[dlid] = self.liststore.append( [dlid, url, 0, '0%', False] )
		
	def __on_pause(self, button):
		(model, iter) = self.treeview.get_selection().get_selected()
		(dlid, paused) = self.liststore.get(iter, 0, 4)
		if paused:
			self.downloadmanager.ResumeDownload(dlid)
			self.liststore.set_value(iter, 4, False)
		else:
			self.downloadmanager.PauseDownload(dlid)
			self.liststore.set_value(iter, 4, True)
			
	def __on_stop(self, button):
		(model, iter) = self.treeview.get_selection().get_selected()
		dlid = self.liststore.get(iter, 0)[0]
		self.downloadmanager.AbortDownload(dlid)
		self.liststore.remove(iter)
		
	def __on_cursor_changed(self, tw):
		if len(tw.get_selection().get_selected_rows()) != 0:
			self.button_pause.set_sensitive(True)
			self.button_stop.set_sensitive(True)
		else:
			self.button_pause.set_sensitive(False)
			self.button_stop.set_sensitive(False)
		
	def __on_download_status(self, dlid, action, progress):
		self.liststore.set_value(self.dlid_to_iter[dlid], 2, progress*100)
		self.liststore.set_value(self.dlid_to_iter[dlid], 3, "%i%%" % (progress*100))
		
	def __on_quit(self, *args):		
		gtk.main_quit()
		
if __name__=='__main__':
	gobject.threads_init()
	dbus.glib.init_threads()
	w = DownloadManagerWindow()
	w.show_all()
	gtk.main()
