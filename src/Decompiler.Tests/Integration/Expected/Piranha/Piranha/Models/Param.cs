using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class Param
	{
		public DateTime Created
		{
			get;
			set;
		}

		[StringLength(0xff)]
		public string Description
		{
			get;
			set;
		}

		public Guid Id
		{
			get;
			set;
		}

		[Required]
		[StringLength(64)]
		public string Key
		{
			get;
			set;
		}

		public DateTime LastModified
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public Param()
		{
		}
	}
}