using JetBrains.Annotations;
using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;

namespace KafkaClient.Logging
{
    public static class Log4NetConfigurator
    {
        public static void Configure([CanBeNull] string logDirectory = null, [CanBeNull] string entryPointName = null)
        {
            var appDomainBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            GlobalContext.Properties["LogDirectory"] = logDirectory ?? Path.Combine(appDomainBaseDirectory, "logs");
            GlobalContext.Properties["EntryPointName"] = entryPointName ?? Assembly.GetEntryAssembly().GetName().Name;
            var thisType = typeof(Log4NetConfigurator);
            using (var rs = thisType.Assembly.GetManifestResourceStream(thisType, "log4net.config"))
                XmlConfigurator.Configure(rs);
        }
    }
}