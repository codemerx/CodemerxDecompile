using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	public abstract class MediaContent
	{
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

		public Guid? Id
		{
			get;
			set;
		}

		protected MediaContent()
		{
		}
	}
}