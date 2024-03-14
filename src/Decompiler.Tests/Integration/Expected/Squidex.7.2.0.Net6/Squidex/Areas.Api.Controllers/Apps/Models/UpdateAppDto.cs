using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class UpdateAppDto
	{
		public string Description
		{
			get;
			set;
		}

		public string Label
		{
			get;
			set;
		}

		public UpdateAppDto()
		{
		}

		[NullableContext(1)]
		public UpdateApp ToCommand()
		{
			return SimpleMapper.Map<UpdateAppDto, UpdateApp>(this, new UpdateApp());
		}
	}
}