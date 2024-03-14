using Piranha.Extend.Fields;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public sealed class Site
	{
		public DateTime? ContentLastModified
		{
			get;
			set;
		}

		public DateTime Created
		{
			get;
			set;
		}

		[StringLength(6)]
		public string Culture
		{
			get;
			set;
		}

		[StringLength(0x100)]
		public string Description
		{
			get;
			set;
		}

		[StringLength(0x100)]
		public string Hostnames
		{
			get;
			set;
		}

		public Guid Id
		{
			get;
			set;
		}

		[StringLength(64)]
		public string InternalId
		{
			get;
			set;
		}

		public bool IsDefault
		{
			get;
			set;
		}

		public DateTime LastModified
		{
			get;
			set;
		}

		public ImageField Logo { get; set; } = new ImageField();

		[StringLength(64)]
		public string SiteTypeId
		{
			get;
			set;
		}

		[Required]
		[StringLength(128)]
		public string Title
		{
			get;
			set;
		}

		public Site()
		{
		}
	}
}