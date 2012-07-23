using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.QueryModel.NHibernate
{
    public class Version
    {
        public Guid Id { get; set; }

        public String QueryModelType { get; set; }
        
        public Int32 CurrentVersion { get; set; }
    }
}
