using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Config.OpenApi
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class QueryParamsProcessor : IOperationProcessor
	{
		private readonly string path;

		public QueryParamsProcessor(string path)
		{
			this.path = path;
		}

		public bool Process(OperationProcessorContext context)
		{
			if (context.get_OperationDescription().get_Path() == this.path && context.get_OperationDescription().get_Method() == "get")
			{
				QueryExtensions.AddQuery(context.get_OperationDescription().get_Operation(), false);
			}
			return true;
		}
	}
}