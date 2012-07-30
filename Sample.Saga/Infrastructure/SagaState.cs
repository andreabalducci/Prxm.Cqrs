using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Saga.Infrastructure
{
	/// <summary>
	/// class that is used to manage the persistent state of a saga
	/// </summary>
	public class SagaState<TId>
	{
		/// <summary>
		/// can be managed automatically by the persistence layer, you can use a composite key here (make it a string and concat your values)
		/// or you can use custom fields you query on
		/// </summary>
		public TId Id { get; set; }

		public bool Completed { get; set; }
	}
}
