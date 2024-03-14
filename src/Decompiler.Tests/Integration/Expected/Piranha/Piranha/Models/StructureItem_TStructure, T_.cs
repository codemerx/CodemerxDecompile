using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public abstract class StructureItem<TStructure, T>
	where TStructure : Structure<TStructure, T>
	where T : StructureItem<TStructure, T>
	{
		public Guid Id
		{
			get;
			set;
		}

		public TStructure Items
		{
			get;
			set;
		}

		public int Level
		{
			get;
			set;
		}

		protected StructureItem()
		{
		}
	}
}