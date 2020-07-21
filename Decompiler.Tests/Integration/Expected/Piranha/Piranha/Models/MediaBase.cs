using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public abstract class MediaBase
	{
		[StringLength(128)]
		public string AltText
		{
			get;
			set;
		}

		[Required]
		[StringLength(0x100)]
		public string ContentType
		{
			get;
			set;
		}

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

		[Required]
		[StringLength(128)]
		public string Filename
		{
			get;
			set;
		}

		public Guid? FolderId
		{
			get;
			set;
		}

		public int? Height
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

		public string PublicUrl
		{
			get;
			set;
		}

		public long Size
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

		public MediaType Type
		{
			get;
			set;
		}

		public int? Width
		{
			get;
			set;
		}

		protected MediaBase()
		{
			base();
			return;
		}
	}
}