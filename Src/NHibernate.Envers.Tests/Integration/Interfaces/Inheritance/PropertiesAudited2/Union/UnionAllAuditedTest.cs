﻿using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.PropertiesAudited2.Union
{
	[TestFixture]
	public class UnionAllAuditedTest : AbstractAllAuditedTest
	{
		public UnionAllAuditedTest(string strategyType) : base(strategyType)
		{
		}
	}
}