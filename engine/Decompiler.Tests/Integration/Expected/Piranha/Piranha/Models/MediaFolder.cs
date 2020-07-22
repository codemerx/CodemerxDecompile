using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class MediaFolder
	{
		public DateTime Created
		{
			get;
			set;
		}

		[StringLength(0x200)]
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
		[StringLength(128)]
		public string Name
		{
			get;
			set;
		}

		public Guid? ParentId
		{
			get;
			set;
		}

		public MediaFolder()
		{
			base();
			return;
		}
	}
}