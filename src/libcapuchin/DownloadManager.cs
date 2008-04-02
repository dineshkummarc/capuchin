
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Capuchin.Logging;
using Capuchin.Xml;

namespace Capuchin
{
    /// <summary>Class that manages downloads</summary>
    public class DownloadManager : IDownloadManager
    {
        public event DownloadManagerStatusHandler DownloadStatus;
        public event DownloadManagerFinishedHandler DownloadFinished;
        
        public delegate void DownloaderDel(object startPoint);
        
        public const int BLOCKING_DOWNLOAD_ID = -1;
        
        internal Dictionary<int, Download> Downloads;
        
        private int downloadsIndex;
        private int blockingDownloadId;
        
        public DownloadManager()
        {
            this.Downloads = new Dictionary<int, Download>();
            this.downloadsIndex = 0; 
        }
        
        /// <summary>Download a file using the DownloadManager</summary>
        public virtual int DownloadFile(string download_url, string download_dest)
        {
            return this.DownloadFile(download_url, download_dest, null, null);
        }
        
        /// <param name="download_url">Url of file to download</param>
        /// <param name="download_dest">Directory where to save the file</param>
        /// <param name="checksumField">Checksum instance of file or null</param>
        /// <returns>
        /// An <see cref="Capuchin.Downloaders.AbstractDownloader" /> instance to
        /// download the given file
        /// </returns>
        internal int DownloadFile (string download_url, string download_dest,
                                   string signature, checksum checksumField)
        {
            Download dl = new Download(this.downloadsIndex, download_url, download_dest, signature, checksumField);
            
            Downloaders.AbstractDownloader downloader = this.GetDownloader(this.downloadsIndex, dl);
            downloader.Status += new Downloaders.StatusHandler( this.OnDownloadStatus );
            downloader.Finished += new Downloaders.FinishedHandler( this.DownloadFinishedCallback );
            
            Thread downloaderThread = new Thread( new ThreadStart(downloader.Download) );
            dl.Downloader = downloaderThread;
            lock (this) {
                this.Downloads.Add( this.downloadsIndex, dl );
                this.downloadsIndex++;
                downloaderThread.Start();
            }
            
            Log.Info("Started downloading file {0} to {1} with id '{2}'",
                     download_url, download_dest, this.downloadsIndex-1);
            
            return (this.downloadsIndex-1);
        }
    
        internal void DownloadFileBlocking (string download_url, string download_dest)
        {
            Download dl = new Download(this.downloadsIndex, download_url, download_dest, null, null);            
            
            Downloaders.AbstractDownloader downloader = this.GetDownloader(BLOCKING_DOWNLOAD_ID, dl);
            downloader.Status += new Downloaders.StatusHandler( this.OnDownloadStatus );
            downloader.Finished += new Downloaders.FinishedHandler( this.DownloadFinishedCallback );
            
            downloader.Download ();
        }
        
        /// <summary>Pause download</summary>
        /// <param name="id">
        /// Download id as returned by <see cref="Capuchin.DownloadManager.DownloadFile" />
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when download id doesn't exist
        /// </exception>
        public virtual void PauseDownload(int id)
        {
            Log.Info("Paused download with id '{0}'", id);            
            
            if (!this.Downloads.ContainsKey (id)) {
                throw new ArgumentOutOfRangeException ("A download with id "+id+" does not exist");
            }
            
            lock (this) {
                // Kill Downloader Thread
                this.Downloads[id].Downloader.Abort();
            }
        }
        
        /// <summary>Abort download</summary>
        /// <param name="id">
        /// Download id as returned by <see cref="Capuchin.DownloadManager.DownloadFile" />
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when download id doesn't exist
        /// </exception>
        public virtual void AbortDownload(int id)
        {
            Log.Info("Aborted download with id '{0}'", id);
            
            if (!this.Downloads.ContainsKey (id)) {
                throw new ArgumentOutOfRangeException ("A download with id "+id+" does not exist");
            }
        
            lock (this) {
                this.PauseDownload(id);
                File.Delete(this.Downloads[id].LocalFile);
                this.Downloads.Remove(id);
            }
        }
        
        /// <summary>Resume download</summary>
        /// <param name="id">
        /// Download id as returned by <see cref="Capuchin.DownloadManager.DownloadFile" />
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when download id doesn't exist
        /// </exception>
        public virtual void ResumeDownload(int id)
        {
            if (!this.Downloads.ContainsKey (id)) {
                throw new ArgumentOutOfRangeException ("A download with id "+id+" does not exist");
            }
            
            // Get file info
            FileInfo f = new FileInfo( this.Downloads[id].LocalFile );
            
            lock (this) {
                // Get Downloader
                Downloaders.AbstractDownloader downloader = this.GetDownloader(id, this.Downloads[id]);
                // FIXME: Do we really need to connect the signals again?
                downloader.Status += new Downloaders.StatusHandler( this.OnDownloadStatus );
                downloader.Finished += new Downloaders.FinishedHandler( this.DownloadFinishedCallback );
                // Start Thread
                Thread downloaderThread = new Thread( new ParameterizedThreadStart(downloader.Download) );
                this.Downloads[id] = new Download ( id,
                    this.Downloads[id].Url,
                    this.Downloads[id].Destination,
                    this.Downloads[id].Signature,
                    this.Downloads[id].Checksum,
                    downloaderThread);
                downloaderThread.Start(f.Length);
            }
            Log.Info("Resuming download with id '{0}'", id);
        }
        
        protected void OnDownloadStatus(int id, double progress, int speed)
        {
            if (DownloadStatus != null)
            {
                DownloadStatus(id, progress, speed);
            }
        }
        
        /// <summary>Emits the finished signal</summary>
        protected void OnDownloadFinished(int id)
        {
            if (DownloadFinished != null) {
                DownloadFinished(id);
            }
        }
            
        private void DownloadFinishedCallback(Downloaders.AbstractDownloader downloader, int id)
        {
            Log.Info("Finished downloading file with id '{0}'", id);
            
            // Don't report finished signal when download is called blocking
            if (id == BLOCKING_DOWNLOAD_ID) return;
            
            lock (this) {
                // Remove Download
                this.Downloads.Remove(id);
            
                // TODO: Maybe dangerous when disconnecting while other download
                // is still running?
                // Disconnect signals
                downloader.Status -= new Downloaders.StatusHandler( this.OnDownloadStatus );
                downloader.Finished -= new Downloaders.FinishedHandler( this.DownloadFinishedCallback );
            }
            
            this.OnDownloadFinished(id);
        }
        
        /// <summary>Returns the appropriate <see cref="Capuchin.Downloaders.AbstractDownloader" /></summary>
        internal Downloaders.AbstractDownloader GetDownloader(int id, Download dl)
        {
            Uri uri = new Uri(dl.Url);
            
            if (uri.Scheme == "http")
            {
                return new Downloaders.HttpDownloader(id, dl);
//            } else if (uri.Scheme == "ftp") {
//                return new Downloaders.FtpDownloader(id, dl);
            } else {
                throw new NotImplementedException("Scheme '"+ uri.Scheme + "' is not supported");                
            }
        }
        
    }
}
