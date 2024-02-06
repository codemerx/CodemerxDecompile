using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Error
{
	[Nullable(0)]
	[NullableContext(2)]
	public class ErrorVM
	{
		public string ErrorCode { get; set; } = "400";

		public string ErrorMessage
		{
			get;
			set;
		}

		public ErrorVM()
		{
		}
	}
}