using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public sealed class Comment
	{
		[Required]
		[StringLength(128)]
		public string Author
		{
			get;
			set;
		}

		[Required]
		public string Body
		{
			get;
			set;
		}

		public Guid ContentId
		{
			get;
			set;
		}

		[Required]
		public DateTime Created
		{
			get;
			set;
		}

		[EmailAddress]
		[Required]
		[StringLength(128)]
		public string Email
		{
			get;
			set;
		}

		public Guid Id
		{
			get;
			set;
		}

		public string IpAddress
		{
			get;
			set;
		}

		public bool IsApproved { get; set; } = true;

		[StringLength(0x100)]
		public string Url
		{
			get;
			set;
		}

		public string UserAgent
		{
			get;
			set;
		}

		[StringLength(128)]
		public string UserId
		{
			get;
			set;
		}

		public Comment()
		{
		}
	}
}