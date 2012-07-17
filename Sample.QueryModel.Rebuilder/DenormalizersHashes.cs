using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.QueryModel.Rebuilder
{
    /// <summary>
    /// holds a reference to the list of available hash informations
    /// the key of the dictionary is the name of the hashed denormalizer class
    /// </summary>
    public class DenormalizersHashes
    {
		public Guid Id;

        public IList<DenormalizerHash> Hashes { get; set; }

        public DenormalizersHashes()
        {
            Hashes = new List<DenormalizerHash>();
        }
    }

    /// <summary>
    /// holds the hash information
    /// </summary>
    public class DenormalizerHash
    {
        /// <summary>
        /// fullname of the class that implememts the denormalizer
        /// </summary>
        public string Name { get; set; }

        public string Hash { get; set; }

        /// <summary>
        /// timestamp at which the hash was generated
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
