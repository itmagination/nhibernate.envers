﻿using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.PropertiesAudited2.Subclass
{
	[TestFixture]
	public class SubclassAllAuditedTest : AbstractAllAuditedTest
	{
		public SubclassAllAuditedTest(string strategyType) : base(strategyType)
		{
		}
	}
}