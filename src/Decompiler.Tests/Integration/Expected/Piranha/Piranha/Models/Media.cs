using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public sealed class Media : MediaBase
	{
		public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

		public IList<MediaVersion> Versions { get; set; } = new List<MediaVersion>();

		public Media()
		{
		}
	}
}