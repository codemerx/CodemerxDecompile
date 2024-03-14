using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public abstract class ContentBase
	{
		public DateTime Created
		{
			get;
			set;
		}

		public Guid Id
		{
			get;
			set;
		}

		public DateTime LastModified
		{
			get;
			set;
		}

		public IList<string> Permissions { get; set; } = new List<string>();

		[StringLength(128)]
		public string Title
		{
			get;
			set;
		}

		[StringLength(64)]
		public string TypeId
		{
			get;
			set;
		}

		protected ContentBase()
		{
		}
	}
}