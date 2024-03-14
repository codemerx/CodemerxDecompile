using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class SchemaScriptsDto
	{
		public string Change
		{
			get;
			set;
		}

		public string Create
		{
			get;
			set;
		}

		public string Delete
		{
			get;
			set;
		}

		public string Query
		{
			get;
			set;
		}

		public string QueryPre
		{
			get;
			set;
		}

		public string Update
		{
			get;
			set;
		}

		public SchemaScriptsDto()
		{
		}

		[NullableContext(1)]
		public ConfigureScripts ToCommand()
		{
			SchemaScripts schemaScript = SimpleMapper.Map<SchemaScriptsDto, SchemaScripts>(this, new SchemaScripts());
			ConfigureScripts configureScript = new ConfigureScripts();
			configureScript.set_Scripts(schemaScript);
			return configureScript;
		}
	}
}