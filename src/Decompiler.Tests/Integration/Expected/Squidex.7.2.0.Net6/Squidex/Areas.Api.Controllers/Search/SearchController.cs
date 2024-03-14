using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.Search.Models;
using Squidex.Domain.Apps.Entities.Search;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Search
{
	[ApiExplorerSettings(GroupName="Search")]
	[Nullable(0)]
	[NullableContext(1)]
	public class SearchController : ApiController
	{
		private readonly ISearchManager searchManager;

		public SearchController(ISearchManager searchManager, ICommandBus commandBus) : base(commandBus)
		{
			this.searchManager = searchManager;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.search" })]
		[HttpGet]
		[ProducesResponseType(typeof(SearchResultDto[]), 200)]
		[Route("apps/{app}/search/")]
		public async Task<IActionResult> GetSearchResults(string app, [Nullable(2)][FromQuery] string query = null)
		{
			SearchResults searchResult = await this.searchManager.SearchAsync(query, base.get_Context(), base.get_HttpContext().get_RequestAborted());
			SearchResultDto[] array = searchResult.Select<SearchResult, SearchResultDto>(new Func<SearchResult, SearchResultDto>(SearchResultDto.FromDomain)).ToArray<SearchResultDto>();
			return this.Ok(array);
		}
	}
}