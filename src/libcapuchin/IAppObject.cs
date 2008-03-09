using System;
using NDesk.DBus;

namespace Capuchin
{
   public enum ActionType {
        UpdatingRepo,
        DownloadingPlugin,
        ExtractingPlugin
    }
    
    public delegate void UpdateFinishedHandler ();
    public delegate void InstallFinishedHandler (string plugin_id);
    public delegate void StatusHandler (ActionType action, string plugin_id, double progress, int speed);
            
    [Interface("org.gnome.Capuchin.AppObject")]
    public interface IAppObject
    {
        event UpdateFinishedHandler UpdateFinished;
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
        string[] GetTags ();
        void Close ();
    }
}
