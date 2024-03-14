using Piranha.Extend;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Fields
{
	public abstract class SimpleField<T> : Field, IEquatable<SimpleField<T>>
	{
		public T Value
		{
			get;
			set;
		}

		protected SimpleField()
		{
		}

		public override bool Equals(object obj)
		{
			SimpleField<T> simpleField = obj as SimpleField<T>;
			if (simpleField == null)
			{
				return false;
			}
			return this.Equals(simpleField);
		}

		public virtual bool Equals(SimpleField<T> obj)
		{
			if (obj == null)
			{
				return false;
			}
			return EqualityComparer<T>.Default.Equals(this.Value, obj.Value);
		}

		public override int GetHashCode()
		{
			if (this.Value.Equals(default(T)))
			{
				return 0;
			}
			return this.Value.GetHashCode();
		}

		public override string GetTitle()
		{
			if (this.Value == null)
			{
				return null;
			}
			string str = this.Value.ToString();
			if (str.Length > 40)
			{
				str = String.Concat(str.Substring(0, 40), "...");
			}
			return str;
		}

		public static bool operator ==(SimpleField<T> field1, SimpleField<T> field2)
		{
			if (field1 != null && field2 != null)
			{
				return field1.Equals(field2);
			}
			if (field1 == null && field2 == null)
			{
				return true;
			}
			return false;
		}

		public static bool operator !=(SimpleField<T> field1, SimpleField<T> field2)
		{
			return !(field1 == field2);
		}
	}
}