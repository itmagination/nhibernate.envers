using System.Collections.Generic;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Event;
using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	public abstract class TestBase
	{
		public readonly static string TestAssembly = Assembly.GetCallingAssembly().GetName().Name;
		protected Cfg.Configuration Cfg { get; private set; }
		protected ISession Session { get; private set; }
		private ISessionFactory SessionFactory { get; set; }
		private IAuditReader _auditReader;

		[SetUp]
		public void BaseSetup()
		{
			Cfg = new Cfg.Configuration();
			Cfg.Configure();
			addMappings();
			AddToConfiguration(Cfg);
			Cfg.IntegrateWithEnvers(new AuditEventListener(), new AttributeConfiguration());
			SessionFactory = Cfg.BuildSessionFactory();
			Session = openSession(SessionFactory);
			Initialize();
			closeSessionAndAuditReader();
			Session = openSession(SessionFactory);
		}

		protected IAuditReader AuditReader()
		{
			return _auditReader ?? (_auditReader = Session.Auditer());
		}

		protected virtual void AddToConfiguration(Cfg.Configuration configuration){}

		private ISession openSession(ISessionFactory sessionFactory)
		{
			var session = sessionFactory.OpenSession();
			session.FlushMode = FlushMode;
			return session;
		}

		[TearDown]
		public void BaseTearDown()
		{
			closeSessionAndAuditReader();
			if (SessionFactory != null)
			{
				SessionFactory.Close();
			}
		}

		protected virtual FlushMode FlushMode
		{
			get { return FlushMode.Auto; }
		}

		protected abstract void Initialize();

		private string nameSpaceAssemblyExtracted()
		{
			var fullNamespace = GetType().Namespace;
			return fullNamespace.Remove(0, TestAssembly.Length + 1);
		}

		protected virtual IEnumerable<string> Mappings
		{
			get
			{
				return new[]
				       	{
				       		nameSpaceAssemblyExtracted() + ".Mapping.hbm.xml"
				       	};
			}
		}

		private void addMappings()
		{
			var ass = Assembly.Load(TestAssembly);
			foreach (var mapping in Mappings)
			{
				Cfg.AddResource(TestAssembly + "." + mapping, ass);
			}
		}

		private void closeSessionAndAuditReader()
		{
			if (Session != null)
			{
				Session.Close();
			}
			_auditReader = null;
		}
	}
}