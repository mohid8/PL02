using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rqim2Lib
{
    public class Logger
    {
        Logger()
        {
            //appender = new log4net.Appender.FileAppender();
            //appender.File=Utils.CreateDateTimeString("Log_", ".log");
            //log.
            logEnabled = true;
            Setup();
        }
        bool logEnabled;

        public static string CreateDateTimeString(string prefix = "", string suffix = "", bool withMS = false)
        {
            var format = "yyyyMMdd_HHmmss";
            if (withMS)
            {
                format = "yyyyMMdd_HHmmss_fffffff";
            }
            return string.Format("{0}{1}{2}", prefix, DateTime.Now.ToString(format), suffix);
        }

        void Setup()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = false;
            roller.File = CreateDateTimeString("Log_", ".txt");
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "1GB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.StaticLogFileName = true;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);

            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;
        }


        private readonly log4net.ILog log = log4net.LogManager.GetLogger
    (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private static readonly Logger l = new Logger();
        public static Logger L
        {
            get
            {
                //if (s == null) { s = new VisionSettings(); }
                return l;
            }
        }


        public void LogInfo(string format, params object[] args)
        {
            if (logEnabled)
            {
                log.Info(string.Format(format, args));
            }
        }

        public void LogInfo(string s)
        {
            if (logEnabled)
            {
                log.Info(s);
            }
        }

    }
}
