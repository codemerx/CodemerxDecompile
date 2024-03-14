using Squidex.Domain.Apps.Core.Apps;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class EditorDto
	{
		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Url
		{
			get;
			set;
		}

		public EditorDto()
		{
		}

		public static EditorDto FromDomain(Editor editor)
		{
			return SimpleMapper.Map<Editor, EditorDto>(editor, new EditorDto());
		}

		public Editor ToEditor()
		{
			return new Editor(this.Name, this.Url);
		}
	}
}