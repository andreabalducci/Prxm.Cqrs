using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.QueryModel.Rebuilder
{
    /// <summary>
    /// a module that give access to the storage of the computed Denormalizers Hashes
    /// </summary>
    public interface IDenormalizersHashesStore
    {
        void Save(DenormalizersHashes hashes);

        DenormalizersHashes Load();
    }
}
