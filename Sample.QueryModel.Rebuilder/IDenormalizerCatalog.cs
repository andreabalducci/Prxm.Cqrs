using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sample.QueryModel.Builder;

namespace Sample.QueryModel.Rebuilder
{
    public interface IDenormalizerCatalog
    {
        IEnumerable<Type> Denormalizers { get; }
    }
}
