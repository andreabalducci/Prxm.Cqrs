using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sample.DebugUi.Infrastructure
{
    public class LogMessage
    {
        [XmlAttribute("logger")]
        public String Logger { get; set; }

        [XmlAttribute("timestamp")]
        public DateTime Timestamp { get; set; }

        [XmlAttribute("level")]
        public String Level { get; set; }

        [XmlAttribute("thread")]
        public Int32 ThreadId { get; set; }

        [XmlElement("message")]
        public String Message { get; set; }
    }
}
