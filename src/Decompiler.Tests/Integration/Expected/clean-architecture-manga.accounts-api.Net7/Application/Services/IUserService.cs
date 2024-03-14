using System;
using System.Runtime.CompilerServices;

namespace Application.Services
{
	[NullableContext(1)]
	public interface IUserService
	{
		string GetCurrentUserId();
	}
}