using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	[AttributeUsage(AttributeTargets.Class)]
	public class BlockTypeAttribute : Attribute
	{
		private bool _isGenericManuallySet;

		private bool _isGeneric = true;

		private string _component = "generic-block";

		public string Category
		{
			get;
			set;
		}

		public string Component
		{
			get
			{
				return this._component;
			}
			set
			{
				this._component = value;
				if (!this._isGenericManuallySet)
				{
					this._isGeneric = value == "generic-block";
				}
			}
		}

		public string Icon
		{
			get;
			set;
		}

		public bool IsGeneric
		{
			get
			{
				return this._isGeneric;
			}
			set
			{
				this._isGeneric = value;
				this._isGenericManuallySet = true;
			}
		}

		public bool IsUnlisted
		{
			get;
			set;
		}

		public string ListTitle
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public BlockTypeAttribute()
		{
		}
	}
}