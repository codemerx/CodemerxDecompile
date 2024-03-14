using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class UpdateClientDto
	{
		public bool? AllowAnonymous
		{
			get;
			set;
		}

		public long? ApiCallsLimit
		{
			get;
			set;
		}

		public long? ApiTrafficLimit
		{
			get;
			set;
		}

		[LocalizedStringLength(20)]
		public string Name
		{
			get;
			set;
		}

		public string Role
		{
			get;
			set;
		}

		public UpdateClientDto()
		{
		}

		[NullableContext(1)]
		public UpdateClient ToCommand(string clientId)
		{
			UpdateClient updateClient = new UpdateClient();
			updateClient.set_Id(clientId);
			return SimpleMapper.Map<UpdateClientDto, UpdateClient>(this, updateClient);
		}
	}
}