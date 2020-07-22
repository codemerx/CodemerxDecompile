using Piranha.Extend;
using System;
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
			base();
			return;
		}

		public override bool Equals(object obj)
		{
			V_0 = obj as SimpleField<T>;
			if (V_0 == null)
			{
				return false;
			}
			return this.Equals(V_0);
		}

		public virtual bool Equals(SimpleField<T> obj)
		{
			if (!SimpleField<T>.op_Inequality(obj, null))
			{
				return false;
			}
			return EqualityComparer<T>.get_Default().Equals(this.get_Value(), obj.get_Value());
		}

		public override int GetHashCode()
		{
			V_0 = this.get_Value();
			V_1 = default(T);
			if (V_0.Equals(V_1))
			{
				return 0;
			}
			return this.get_Value().GetHashCode();
		}

		public override string GetTitle()
		{
			if (this.get_Value() == null)
			{
				return null;
			}
			V_0 = this.get_Value().ToString();
			if (V_0.get_Length() > 40)
			{
				V_0 = String.Concat(V_0.Substring(0, 40), "...");
			}
			return V_0;
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
			return !SimpleField<T>.op_Equality(field1, field2);
		}
	}
}