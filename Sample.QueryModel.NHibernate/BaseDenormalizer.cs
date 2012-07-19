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
        /// <summary>
        /// static 
        /// </summary>
        protected BaseDenormalizer(ICommandQueue commandQueue)
        {
            //if (isInited) return;
            // lock (_sSyncroot)
            // {
            //     if (!isInited)
            //     {
            //         NHibernateHelper.UpdateDatabase();
            //         //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            //         //isInited = true;
            //         //IList<Version> allVersions;

            //         //List<Type> allModifiedDenormalizer = new List<Type>();

            //         //using (ISession session = NHibernateHelper.OpenSession())
            //         //{
            //         //    allVersions = session.Query<Version>().ToList();
            //         //}
            //         ////Scan all denormalizer of this assembly.
            //         //foreach (var denType in Assembly.GetExecutingAssembly()
            //         //    .GetTypes()
            //         //    .Where(t => typeof(BaseDenormalizer).IsAssignableFrom(t) && !t.IsAbstract))
            //         //{
            //         //    //load all versions.
            //         //    var versionAttribute = denType
            //         //        .GetCustomAttributes(typeof(CurrentDenormalizerVersionAttribute), false)
            //         //        .OfType<CurrentDenormalizerVersionAttribute>()
            //         //        .SingleOrDefault();
            //         //    if (versionAttribute == null)
            //         //    {
            //         //        versionAttribute = new CurrentDenormalizerVersionAttribute(0);

            //         //    }
            //         //    Version version = allVersions.SingleOrDefault(v => v.Id == denType.FullName);
            //         //    if (version == null)
            //         //    {
            //         //        version = new Version();
            //         //        version.Id = denType.FullName;
            //         //        allVersions.Add(version);
            //         //    }
            //         //    if (version.CurrentVersion != versionAttribute.CurrentValue)
            //         //    {
            //         //        allModifiedDenormalizer.Add(denType);
            //         //        version.CurrentVersion = versionAttribute.CurrentValue;
            //         //    }
            //         //}
            //         //using (ISession session = NHibernateHelper.OpenSession())
            //         //using (session.BeginTransaction())
            //         //{
            //         //    foreach (var version in allVersions)
            //         //    {
            //         //        session.SaveOrUpdate(version);
            //         //    }
            //         //    session.Transaction.Commit();
            //         //}
            //         //if (allModifiedDenormalizer.Count > 0)
            //         //{

            //         //    //ask to reinit all changed denormalizer, need to interface with replayer event.
            //         //    foreach (var modifiedDenormalizer in allModifiedDenormalizer)
            //         //    {
            //         //        AskForSpecificHandlerReplayCommand cmd = new AskForSpecificHandlerReplayCommand(
            //         //            Guid.NewGuid(),
            //         //            modifiedDenormalizer.FullName + ", " + modifiedDenormalizer.Assembly.GetName().Name);
            //         //        commandQueue.Enqueue(cmd); //TODO: understand why this command is not fired.
            //         //    }
            //         //}
            //     }
            // } 

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
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.SaveOrUpdate(obj);
                    tx.Commit();
                }
            }
        }

        private Version GetDatabaseVersionForThisDenormalizer()
        {
            Version version = null;
            ExecuteInSession(session =>
            {
                version = session.Query<Version>().SingleOrDefault(v => v.Id == this.GetType().FullName);
                if (version == null)
                {
                    version = new Version() { Id = this.GetType().FullName, CurrentVersion = -1 };
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
