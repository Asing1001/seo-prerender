using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using log4net;

namespace Seo.Prerender.Helpers
{
    internal class FileWatchHelper
    {
        public static void WatchFile(string filePath, Action callBack)
        {      
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            watcher.Path = Path.GetDirectoryName(filePath);
            watcher.Filter = Path.GetFileName(filePath);
            watcher.Changed += (sender, args) => (new Thread(() =>
            {
                Thread.Sleep(5000);
                callBack();
            })).Start();

            //Begin watching.
            watcher.EnableRaisingEvents = true; 
        }

    }
}
