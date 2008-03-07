using System;
using NDesk.DBus;

namespace Capuchin
{
    public delegate void DownloadManagerStatusHandler(int id, double progress, int speed);
    public delegate void DownloadManagerFinishedHandler(int id);
    
    [Interface("org.gnome.Capuchin.DownloadManager")]
    public interface IDownloadManager
    {
        event DownloadManagerStatusHandler DownloadStatus;
        event DownloadManagerFinishedHandler DownloadFinished;
        int DownloadFile(string download_url, string download_dest);
        void PauseDownload(int id);
        void AbortDownload(int id);
        void ResumeDownload(int id);
    }
    
}
