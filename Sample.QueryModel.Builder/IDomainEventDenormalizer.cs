using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;

namespace Sample.QueryModel.Builder
{
	/// <summary>
	/// interface that marks a querymodel builder event handler, it will be used in the replay action to rebuild the views
	/// todo: maybe we can refactor this decorate the denormalizers with an empty interface or an attribute
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IDomainEventDenormalizer : IDomainEventHandler 
	{
	}
}
