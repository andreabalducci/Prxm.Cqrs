using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proximo.Cqrs.Core.Bus
{
	public interface IStartableBus
	{
		void Start();
	}
}
