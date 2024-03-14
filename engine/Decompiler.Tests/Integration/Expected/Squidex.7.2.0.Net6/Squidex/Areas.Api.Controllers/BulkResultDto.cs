using Microsoft.AspNetCore.Http;
using Squidex.Domain.Apps.Entities;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class BulkResultDto
	{
		[Obsolete("Use 'id' field now.")]
		public DomainId? ContentId
		{
			get
			{
				return this.Id;
			}
		}

		public ErrorDto Error
		{
			get;
			set;
		}

		public DomainId? Id
		{
			get;
			set;
		}

		public int JobIndex
		{
			get;
			set;
		}

		public BulkResultDto()
		{
		}

		[NullableContext(1)]
		public static BulkResultDto FromDomain(BulkUpdateResultItem result, HttpContext httpContext)
		{
			ErrorDto item1;
			Exception exception = result.get_Exception();
			if (exception != null)
			{
				item1 = ApiExceptionConverter.ToErrorDto(exception, httpContext).Item1;
			}
			else
			{
				item1 = null;
			}
			ErrorDto errorDto = item1;
			return SimpleMapper.Map<BulkUpdateResultItem, BulkResultDto>(result, new BulkResultDto()
			{
				Error = errorDto
			});
		}
	}
}