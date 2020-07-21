using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	public class BinaryMediaContent : MediaContent
	{
		public byte[] Data
		{
			get;
			set;
		}

		public BinaryMediaContent()
		{
			base();
			return;
		}
	}
}