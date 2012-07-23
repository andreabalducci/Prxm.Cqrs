using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sample.DebugUi.Infrastructure
{
    public class LogMessage
    {
        public LogMessage() 
        {
            Properties = new Dictionary<string, string>();
        }

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

        [XmlElement("exception")]
        public String Exception { get; set; }

        /// <summary>
        /// this is a specific property, if in thread context the logger set the op_type property
        /// it gets collected here.
        /// </summary>
        [XmlElement("optype")]
        public String OpType { get; set; }

        /// <summary>
        /// This is the list of detailed properties of the log
        /// </summary>
        public Dictionary<String, String> Properties { get; set; }
        
    }
}
