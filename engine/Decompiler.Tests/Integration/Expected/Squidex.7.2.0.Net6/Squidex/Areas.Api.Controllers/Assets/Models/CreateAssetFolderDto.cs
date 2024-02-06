using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CreateAssetFolderDto
	{
		[LocalizedRequired]
		public string FolderName
		{
			get;
			set;
		}

		public DomainId ParentId
		{
			get;
			set;
		}

		public CreateAssetFolderDto()
		{
		}

		public CreateAssetFolder ToCommand()
		{
			return SimpleMapper.Map<CreateAssetFolderDto, CreateAssetFolder>(this, new CreateAssetFolder());
		}
	}
}