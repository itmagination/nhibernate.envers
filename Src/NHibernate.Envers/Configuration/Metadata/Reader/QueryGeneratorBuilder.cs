﻿using System;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Strategy;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	/// <summary>
	/// Builds query generators, for reading collection middle tables, along with any related entities.
	/// The related entities information can be added gradually, and when complete, the query generator can be built.
	/// </summary>
	public sealed class QueryGeneratorBuilder 
	{
		private readonly AuditEntitiesConfiguration _verEntCfg;
		private readonly IAuditStrategy _auditStrategy;
		private readonly MiddleIdData _referencingIdData;
		private readonly string _auditMiddleEntityName;
		private readonly IList<MiddleIdData> _idDatas;

		public QueryGeneratorBuilder(AuditEntitiesConfiguration verEntCfg, IAuditStrategy auditStrategy,
							  MiddleIdData referencingIdData, string auditMiddleEntityName) 
		{
			_verEntCfg = verEntCfg;
			_auditStrategy = auditStrategy;
			_referencingIdData = referencingIdData;
			_auditMiddleEntityName = auditMiddleEntityName;

			_idDatas = new List<MiddleIdData>();
		}

		public void AddRelation(MiddleIdData idData) 
		{
			_idDatas.Add(idData);
		}

		public IRelationQueryGenerator Build(IEnumerable<MiddleComponentData> componentDatas)
		{
			if (_idDatas.Count == 0) 
			{
				return new OneEntityQueryGenerator(_verEntCfg, _auditStrategy, _auditMiddleEntityName, _referencingIdData, componentDatas);
			}
			if (_idDatas.Count == 1)
			{
				if (_idDatas[0].IsAudited()) 
				{
					return new TwoEntityQueryGenerator(_verEntCfg, _auditStrategy, _auditMiddleEntityName, _referencingIdData,
													   _idDatas[0], componentDatas);
				}
				return new TwoEntityOneAuditedQueryGenerator(_verEntCfg, _auditStrategy, _auditMiddleEntityName, _referencingIdData,
															 _idDatas[0], componentDatas);
			}
			if (_idDatas.Count == 2) 
			{
				// All entities must be audited.
				if (!_idDatas[0].IsAudited() || !_idDatas[1].IsAudited()) 
				{
					throw new MappingException("Ternary relations using @Audited(TargetAuditMode = NotAudited) are not supported.");
				}

				return new ThreeEntityQueryGenerator(_verEntCfg, _auditStrategy, _auditMiddleEntityName, _referencingIdData,
													 _idDatas[0], _idDatas[1], componentDatas);
			}
			throw new NotSupportedException("Illegal number of related entities.");
		}


		/// <summary>
		/// Current index of data in the array, which will be the element of a list, returned when executing a query
		/// generated by the built query generator.
		/// </summary>
		public int CurrentIndex
		{
			get { return _idDatas.Count; }
		}
	}
}
