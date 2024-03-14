using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AddWorkflowDto
	{
		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		public AddWorkflowDto()
		{
		}

		public AddWorkflow ToCommand()
		{
			AddWorkflow addWorkflow = new AddWorkflow();
			addWorkflow.set_Name(this.Name);
			return addWorkflow;
		}
	}
}