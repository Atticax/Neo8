using System.Net;
using Netsphere.Common.Configuration;

namespace PatchManager.Config
{
    public class AppOptions
    {
        public int WorkerThreads { get; set; }
        public DatabaseOption Database { get; set; }
        public LoggerOptions Logging { get; set; }
        public string StorageDir { get; set; }
    }
}
