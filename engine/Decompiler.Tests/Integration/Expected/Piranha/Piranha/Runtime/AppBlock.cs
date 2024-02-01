using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public sealed class AppBlock : AppDataItem
	{
		public string Category
		{
			get;
			set;
		}

		public string Component
		{
			get;
			set;
		}

		public BlockDisplayMode Display
		{
			get;
			set;
		}

		public string Icon
		{
			get;
			set;
		}

		public bool IsGeneric
		{
			get;
			set;
		}

		public bool IsUnlisted
		{
			get;
			set;
		}

		public IList<System.Type> ItemTypes { get; set; } = new List<System.Type>();

		public string ListTitleField
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public bool UseCustomView
		{
			get;
			set;
		}

		public AppBlock()
		{
		}
	}
}