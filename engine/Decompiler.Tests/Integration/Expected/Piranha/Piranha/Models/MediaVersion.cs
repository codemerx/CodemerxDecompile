using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class MediaVersion
	{
		public string FileExtension
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

		public long Size
		{
			get;
			set;
		}

		public int Width
		{
			get;
			set;
		}

		public MediaVersion()
		{
		}
	}
}