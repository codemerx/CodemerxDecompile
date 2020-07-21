using System;

namespace Piranha.Models
{
	[Serializable]
	public class PostArchive : PostArchive<DynamicPost>
	{
		public PostArchive()
		{
			base();
			return;
		}
	}
}