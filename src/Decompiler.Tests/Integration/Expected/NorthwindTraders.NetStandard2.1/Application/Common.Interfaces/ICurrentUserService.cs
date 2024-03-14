using System;

namespace Northwind.Application.Common.Interfaces
{
	public interface ICurrentUserService
	{
		bool IsAuthenticated
		{
			get;
		}

		string UserId
		{
			get;
		}
	}
}