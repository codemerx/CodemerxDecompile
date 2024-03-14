using Squidex.Domain.Apps.Entities.Search;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Search.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public class SearchResultDto : Resource
	{
		[Nullable(2)]
		public string Label
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[LocalizedRequired]
		public SearchResultType Type
		{
			get;
			set;
		}

		public SearchResultDto()
		{
		}

		protected SearchResultDto CreateLinks(SearchResult searchResult)
		{
			base.AddGetLink("url", searchResult.get_Url(), null);
			return this;
		}

		public static SearchResultDto FromDomain(SearchResult searchResult)
		{
			return SimpleMapper.Map<SearchResult, SearchResultDto>(searchResult, new SearchResultDto()).CreateLinks(searchResult);
		}
	}
}