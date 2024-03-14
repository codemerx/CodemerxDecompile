using System;

namespace Northwind.Application.Common.Exceptions
{
	public class NotFoundException : Exception
	{
		public NotFoundException(string name, object key) : base(String.Format("Entity \"{0}\" ({1}) was not found.", (object)name, key))
		{
		}
	}
}