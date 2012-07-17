using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.QueryModel.Rebuilder;
using Sample.QueryModel.Builder.Denormalizers.Inventory;

namespace Sample.Server.Support
{
	/// <summary>
	/// test class that actually lists all the denormalizers we want to automatically rebuild at startup
	/// todo: implement a real deiscovery mechanic
	/// </summary>
	public class DenormalizersDemoCatalog : IDenormalizerCatalog
	{
		public IEnumerable<Type> Denormalizers
		{
			get
			{ 
				yield return typeof(InventoryItemDenormalizer);
			}
		}
	}
}
