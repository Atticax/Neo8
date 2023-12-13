using System;
using System.Net;
using Netsphere.Common.Configuration;

namespace Launcher
{
    public class AppOptions
    {
        public IPEndPoint AuthEndpoint { get; set; }
        public IPEndPoint GameEndpoint { get; set; }
        public int WorkerThreads { get; set; }
        public LoggerOptions Logging { get; set; }
        public string DataDir { get; set; }
    }
}
