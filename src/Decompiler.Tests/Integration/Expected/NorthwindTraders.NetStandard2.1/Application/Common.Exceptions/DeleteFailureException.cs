using System;

namespace Northwind.Application.Common.Exceptions
{
	public class DeleteFailureException : Exception
	{
		public DeleteFailureException(string name, object key, string message) : base(String.Format("Deletion of entity \"{0}\" ({1}) failed. {2}", (object)name, key, message))
		{
		}
	}
}