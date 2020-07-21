using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class Alias
	{
		[Required]
		[StringLength(0x100)]
		public string AliasUrl
		{
			get;
			set;
		}

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

		[Required]
		[StringLength(0x100)]
		public string RedirectUrl
		{
			get;
			set;
		}

		public Guid SiteId
		{
			get;
			set;
		}

		public RedirectType Type
		{
			get;
			set;
		}

		public Alias()
		{
			this.u003cTypeu003ek__BackingField = 1;
			base();
			return;
		}
	}
}