using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	public class StreamMediaContent : MediaContent
	{
		public Stream Data
		{
			get;
			set;
		}

		public StreamMediaContent()
		{
			base();
			return;
		}
	}
}