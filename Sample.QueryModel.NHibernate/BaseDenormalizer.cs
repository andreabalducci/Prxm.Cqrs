using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proximo.Cqrs.Server.Eventing;
using NHibernate;
using System.Reflection;
using NHibernate.Linq;
using Sample.Commands.System;
using Proximo.Cqrs.Core.Commanding;
using Sample.Commands.Inventory;
using Sample.Server.Core.Initialization;

namespace Sample.QueryModel.NHibernate
{
    /// <summary>
    /// Base class for denormalizing into QueryModel.
    /// </summary>
    public abstract class BaseDenormalizer : IDomainEventHandler, IReplayable
    {

        private static Boolean isInited;
        private static Object _sSyncroot = new object();

        /// <summary>
        /// 
        /// </summary>
        static BaseDenormalizer()
        {
            //just init database
            NHibernateHelper.UpdateDatabase();
        }
   
        protected void ExecuteInSession(Action<ISession> act)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {

                    //TODO: logging, exceptionhandling.
                    act(session);
                    tx.Commit();
                }
            }
        }

        protected T ExecuteInSession<T>(Func<ISession, T> act)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {

                    //TODO: logging, exceptionhandling.
                    var retValue = act(session);
                    tx.Commit();
                    return retValue;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected T GetById<T>(Object id)
        {
            T retValue;
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    retValue = session.Get<T>(id);
                    tx.Commit();
                }
            }
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected void SaveOrUpdate(Object obj)
        {
           ExecuteInSession(s => s.SaveOrUpdate(obj));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected void Save(Object obj)
        {
            ExecuteInSession(s => s.Save(obj));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        protected void Update(Object obj)
        {
            ExecuteInSession(s => s.Update(obj));
        }

        private Version GetDatabaseVersionForThisDenormalizer()
        {
            Version version = null;
            ExecuteInSession(session =>
            {
                version = session.Query<Version>().SingleOrDefault(v => v.QueryModelType == this.GetType().FullName);
                if (version == null)
                {
                    version = new Version() { QueryModelType = this.GetType().FullName, CurrentVersion = -1 };
                    session.Save(version);
                }
            });
            return version;
        }

        private DenormalizerVersionAttribute GetVersionAttribute()
        {
            return this.GetType()
                   .GetCustomAttributes(typeof(DenormalizerVersionAttribute), false)
                   .OfType<DenormalizerVersionAttribute>()
                   .SingleOrDefault();
        }

        /// <summary>
        /// Explicitly tells to the engine that it should reply all events checking 
        /// the version attribute.
        /// </summary>
        /// <returns></returns>
        public bool ShouldReplay()
        {
            Version currentVersionInDb = GetDatabaseVersionForThisDenormalizer();
            DenormalizerVersionAttribute attribute = GetVersionAttribute();
            //if the attribute is not present, denormalizer is not interested in replay
            return attribute != null && attribute.Version > currentVersionInDb.CurrentVersion;
        }

        public void StartReplay()
        {
            ExecuteInSession(session =>
            {
                foreach (var type in DenormalizeTypeList)
                {
                    session.CreateQuery("delete from " + type.Name).ExecuteUpdate();
                }
            });
        }

        public void EndReplay()
        {
            ExecuteInSession(session =>
            {
                Version currentVersionInDb = GetDatabaseVersionForThisDenormalizer();
                DenormalizerVersionAttribute attribute = GetVersionAttribute();
                currentVersionInDb.CurrentVersion = attribute.Version;
                session.Update(currentVersionInDb);
            });
        }

        protected abstract Type[] DenormalizeTypeList { get; }
    }
}
