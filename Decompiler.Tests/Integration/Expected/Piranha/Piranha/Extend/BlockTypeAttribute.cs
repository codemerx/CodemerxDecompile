using System;
using System.Runtime.CompilerServices;

namespace Piranha.Extend
{
	[AttributeUsage(AttributeTargets.Class)]
	public class BlockTypeAttribute : Attribute
	{
		private bool _isGenericManuallySet;

		private bool _isGeneric;

		private string _component;

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
					this._isGeneric = String.op_Equality(value, "generic-block");
				}
				return;
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
				return;
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
			this._isGeneric = true;
			this._component = "generic-block";
			base();
			return;
		}
	}
}