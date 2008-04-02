/*
 * This file is part of Capuchin
 * 
 * Copyright (C) Sebastian Pölsterl 2008 <marduk@k-d-w.org>
 * 
 * Capuchin is free software.
 * 
 * You may redistribute it and/or modify it under the terms of the
 * GNU General Public License, as published by the Free Software
 * Foundation; either version 2 of the License, or (at your option)
 * any later version.
 * 
 * Capuchin is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Capuchin.  If not, write to:
 * 	The Free Software Foundation, Inc.,
 * 	51 Franklin Street, Fifth Floor
 * 	Boston, MA  02110-1301, USA.
 */

using System;
using NDesk.DBus;

namespace Capuchin
{
   public enum ActionType {
        UpdatingRepo,
        DownloadingPlugin,
        ExtractingPlugin
    }
    
    public delegate void InstallFinishedHandler (string plugin_id);
    public delegate void StatusHandler (ActionType action, string plugin_id, double progress, int speed);
            
    [Interface("org.gnome.Capuchin.AppObject")]
    public interface IAppObject
    {
        event InstallFinishedHandler InstallFinished;
        event StatusHandler Status;
        
        string GetApplicationName ();
        void Update (bool force_update);
        void Install (string plugin_id);
        string[] GetAvailablePlugins ();
        string[] GetAvailableUpdates (string[][] plugins);
        string[] GetPluginsWithTag (string tag);
        string GetPluginName (string plugin_id);
        string GetPluginDescription (string plugin_id);
        string GetPluginChanges (string plugin_id, string version);
        string[] GetPluginTags (string plugin_id);
        string[] GetPluginAuthor (string plugin_id);
		string GetPluginVersion (string plugin_id);
        string[] GetTags ();
        void Close ();
    }
}
