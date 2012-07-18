using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample.Saga.Infrastructure
{
	/// <summary>
	/// a generic interface to load and write sagas
	/// </summary>
	public interface ISagaRepository
	{
		/// <summary>
		/// loads the curret execution state of a saga
		/// </summary>
		/// <param name="params">Key,Value pairs that are the parameters used to uniquely identify the saga</param>
		/// <returns></returns>
		SagaState Load(IDictionary<string, object> @params);

		/// <summary>
		/// save the saga state
		/// </summary>
		/// <param name="state"></param>
		void Save(SagaState state);

		/// <summary>
		/// The saga ended and can be discarded form the database
		/// </summary>
		/// <param name="state"></param>
		void Remove(SagaState state);
	}
}
