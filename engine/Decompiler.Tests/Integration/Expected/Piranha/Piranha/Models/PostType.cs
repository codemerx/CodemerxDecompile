using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public sealed class PostType : ContentTypeBase
	{
		public bool UseBlocks { get; set; } = true;

		public bool UseExcerpt { get; set; } = true;

		public bool UsePrimaryImage { get; set; } = true;

		public PostType()
		{
		}

		public void Ensure()
		{
			if ((
				from r in base.Regions
				select r.Id).Distinct<string>().Count<string>() != base.Regions.Count)
			{
				throw new InvalidOperationException(String.Concat("Region Id not unique for page type ", base.Id));
			}
			foreach (RegionType region in base.Regions)
			{
				region.Title = region.Title ?? region.Id;
				if ((
					from f in region.Fields
					select f.Id).Distinct<string>().Count<string>() != region.Fields.Count)
				{
					throw new InvalidOperationException(String.Concat("Field Id not unique for page type ", base.Id));
				}
				foreach (FieldType field in region.Fields)
				{
					field.Id = field.Id ?? "Default";
					field.Title = field.Title ?? field.Id;
				}
			}
		}
	}
}