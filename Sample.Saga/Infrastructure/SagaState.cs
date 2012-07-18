using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Saga.Infrastructure
{
	/// <summary>
	/// class that is used to manage the persistent state of a saga
	/// </summary>
	public class SagaState
	{
		public bool Completed { get; set; }
	}
}
