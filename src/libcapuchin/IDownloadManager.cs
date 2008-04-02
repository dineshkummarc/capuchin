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
