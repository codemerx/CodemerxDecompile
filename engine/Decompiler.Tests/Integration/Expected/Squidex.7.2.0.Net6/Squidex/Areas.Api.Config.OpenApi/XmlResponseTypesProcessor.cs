using Namotion.Reflection;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Squidex.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Squidex.Areas.Api.Config.OpenApi
{
	public sealed class XmlResponseTypesProcessor : IOperationProcessor
	{
		[Nullable(1)]
		private readonly static Regex ResponseRegex;

		static XmlResponseTypesProcessor()
		{
			XmlResponseTypesProcessor.ResponseRegex = new Regex("(?<Code>[0-9]{3})[\\s]*=((&gt;)|>)[\\s]*(?<Description>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		}

		public XmlResponseTypesProcessor()
		{
		}

		[NullableContext(1)]
		public bool Process(OperationProcessorContext context)
		{
			OpenApiResponse openApiResponse;
			bool flag;
			OpenApiOperation operation = context.get_OperationDescription().get_Operation();
			string xmlDocsTag = XmlDocsExtensions.GetXmlDocsTag(context.get_MethodInfo(), "returns", null);
			if (!string.IsNullOrWhiteSpace(xmlDocsTag))
			{
				using (IEnumerator<Match> enumerator = XmlResponseTypesProcessor.ResponseRegex.Matches(xmlDocsTag).OfType<Match>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Match current = enumerator.Current;
						string value = current.Groups["Code"].Value;
						if (!operation.get_Responses().TryGetValue(value, out openApiResponse))
						{
							openApiResponse = new OpenApiResponse();
							operation.get_Responses()[value] = openApiResponse;
						}
						string str = current.Groups["Description"].Value;
						if (!str.Contains("=&gt;", StringComparison.Ordinal))
						{
							openApiResponse.set_Description(str);
						}
						else
						{
							Squidex.Infrastructure.ThrowHelper.InvalidOperationException("Description not formatted correcly.");
							flag = false;
							return flag;
						}
					}
					return true;
				}
				return flag;
			}
			return true;
		}
	}
}