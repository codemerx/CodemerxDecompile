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

		public IList<string> Permissions
		{
			get;
			set;
		}

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
			this.u003cPermissionsu003ek__BackingField = new List<string>();
			base();
			return;
		}
	}
}