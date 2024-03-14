using System;
using System.Runtime.CompilerServices;

namespace Piranha.Security
{
	public class PermissionItem
	{
		public string Category
		{
			get;
			set;
		}

		public bool IsInternal
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public PermissionItem()
		{
		}
	}
}