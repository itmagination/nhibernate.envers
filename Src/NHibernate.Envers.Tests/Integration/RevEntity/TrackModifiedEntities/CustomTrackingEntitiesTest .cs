﻿using System.Collections.Generic;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.RevEntity.TrackModifiedEntities
{
	[TestFixture]
	public class CustomTrackingEntitiesTest : TestBase
	{
		private int steId;
		private int siteId;

		protected override void Initialize()
		{
			var ste = new StrTestEntity{Str = "x"};
			var site = new StrIntTestEntity{Str = "y", Number = 1};

			// Revision 1 - Adding two entities
			using (var tx = Session.BeginTransaction())
			{
				steId = (int)Session.Save(ste);
				siteId = (int)Session.Save(site);
				tx.Commit();
			}

			// Revision 2 - Modifying one entity
			using (var tx = Session.BeginTransaction())
			{
				site.Number = 2;
				tx.Commit();
			}

			// Revision 3 - Deleting both entities
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(ste);
				Session.Delete(site);
				tx.Commit();
			}
		}

		[Test]
		public void ShouldTrackAddedEntities()
		{
			var steDescriptor = new ModifiedEntityNameEntity {EntityName = typeof (StrTestEntity).FullName};
			var siteDescriptor = new ModifiedEntityNameEntity {EntityName = typeof (StrIntTestEntity).FullName};

			var ctre = AuditReader().FindRevision<CustomTrackingRevisionEntity>(1);

			ctre.ModifiedEntityNames
				.Should().Have.SameValuesAs(steDescriptor, siteDescriptor);
		}

		[Test]
		public void ShouldTrackModifiedEntities()
		{
			var siteDescriptor = new ModifiedEntityNameEntity { EntityName = typeof(StrIntTestEntity).FullName };

			var ctre = AuditReader().FindRevision<CustomTrackingRevisionEntity>(2);

			ctre.ModifiedEntityNames
				.Should().Have.SameValuesAs(siteDescriptor);
		}

		[Test]
		public void ShouldTrackDeletedEntities()
		{
			var steDescriptor = new ModifiedEntityNameEntity { EntityName = typeof(StrTestEntity).FullName };
			var siteDescriptor = new ModifiedEntityNameEntity { EntityName = typeof(StrIntTestEntity).FullName };

			var ctre = AuditReader().FindRevision<CustomTrackingRevisionEntity>(3);

			ctre.ModifiedEntityNames
				.Should().Have.SameValuesAs(steDescriptor, siteDescriptor);
		}

		[Test]
		[ExpectedException(typeof(AuditException))]
		public void ShouldThrowFindEntitiesChangedInRevision()
		{
			AuditReader().FindEntitiesChangedInRevision(1);
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml", "Entities.RevEntity.TrackModifiedEntities.CustomTrackingRevisionEntity.hbm.xml" }; }
		}
	}
}