using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LogVisualizer.Infrastructure
{
    public class LogMessage
    {
        public LogMessage() 
        {
            Properties = new Dictionary<string, string>();
        }

        public Int32 NumericLevel { get; set; }

        /// <summary>
        /// this is the raw content of the logger
        /// </summary>
        public String FullText { get; set; }

        public String Logger { get; set; }

        public DateTime Timestamp { get; set; }

        public String Level { get; set; }

        public String ThreadId { get; set; }

        public String Message { get; set; }

        public String Username { get; set; }

        public String Exception { get; set; }

        /// <summary>
        /// this is a specific property, if in thread context the logger set the op_type property
        /// it gets collected here.
        /// </summary>
        public String OpType { get; set; }

        public String OpTypeId { get; set; }

        /// <summary>
        /// This is the list of detailed properties of the log
        /// </summary>
        public Dictionary<String, String> Properties { get; set; }
        
    }
}
