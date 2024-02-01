using System;

namespace Piranha.Extend
{
	public abstract class Field : IField
	{
		protected Field()
		{
		}

		public abstract string GetTitle();
	}
}