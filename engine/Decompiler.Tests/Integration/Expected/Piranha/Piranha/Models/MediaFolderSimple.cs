using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class MediaFolderSimple
	{
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

		public MediaFolderSimple()
		{
			base();
			return;
		}
	}
}