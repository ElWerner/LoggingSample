using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using LoggingSample_Logs_DAL.Context;
using LoggingSample_Logs_DAL.Entities;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace LoggingSample_BLL.LogTargets
{
    [Target("XmlTarget")]
    public sealed class XmlTarget : AsyncTaskTarget
    {
        private object _padlock = new object();

        public XmlTarget()
        {
            this.Host = Environment.MachineName;
        }

        [RequiredParameter]
        public string Host { get; set; }

        protected override async Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {
            string currentDate = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\" + currentDate + ".xml";

            lock (_padlock)
            {
                if (!File.Exists(filepath))
                {
                    XmlWriter xmlWriter = XmlWriter.Create(filepath);
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("logs");
                    xmlWriter.Dispose();
                }

                XDocument xmlDoc = XDocument.Load(filepath);

                XElement xmlElement =new XElement("log");
                xmlElement.Add(new XAttribute("MachineName", this.Host),
                    new XAttribute("Exception", logEvent.Exception?.ToString() ),
                    new XAttribute("LoggerName", logEvent.LoggerName),
                    new XAttribute("Level", logEvent.Level.ToString()),
                    new XAttribute("Message", logEvent.Message),
                    new XAttribute("MessageSource", logEvent.CallerFilePath),
                    new XAttribute("TimeStamp", logEvent.TimeStamp));

                xmlDoc.Root.Add(xmlElement);
                xmlDoc.Save(filepath);
            }
        }
    }
}
