using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class ChangeCategoryDto
	{
		public string Name
		{
			get;
			set;
		}

		public ChangeCategoryDto()
		{
		}

		[NullableContext(1)]
		public ChangeCategory ToCommand()
		{
			return SimpleMapper.Map<ChangeCategoryDto, ChangeCategory>(this, new ChangeCategory());
		}
	}
}