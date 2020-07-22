using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public sealed class FieldType
	{
		public string Description
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public FieldOption Options
		{
			get;
			set;
		}

		public string Placeholder
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public FieldType()
		{
			base();
			return;
		}
	}
}