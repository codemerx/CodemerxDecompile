using Piranha;
using Piranha.Extend;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Piranha.Extend.Fields
{
	[FieldType(Name="Select", Shorthand="Select", Component="select-field")]
	public class SelectField<T> : SelectFieldBase, IEquatable<SelectField<T>>
	where T : struct
	{
		private readonly static List<SelectFieldItem> _items;

		private readonly static object Mutex;

		private static bool IsInitialized;

		public override Type EnumType
		{
			get
			{
				return typeof(T);
			}
		}

		public override string EnumValue
		{
			get
			{
				return this.Value.ToString();
			}
			set
			{
				this.Value = (T)Enum.Parse(typeof(T), value);
			}
		}

		public override List<SelectFieldItem> Items
		{
			get
			{
				this.InitMetaData();
				return SelectField<T>._items;
			}
		}

		public T Value
		{
			get;
			set;
		}

		static SelectField()
		{
			SelectField<T>._items = new List<SelectFieldItem>();
			SelectField<T>.Mutex = new Object();
		}

		public SelectField()
		{
		}

		public override bool Equals(object obj)
		{
			SelectField<T> selectField = obj as SelectField<T>;
			if (selectField == null)
			{
				return false;
			}
			return this.Equals(selectField);
		}

		public virtual bool Equals(SelectField<T> obj)
		{
			if (obj == null)
			{
				return false;
			}
			return EqualityComparer<T>.Default.Equals(this.Value, obj.Value);
		}

		private string GetEnumTitle(Enum val)
		{
			MemberInfo[] member = typeof(T).GetMember(val.ToString());
			if (member != null && member.Length != 0)
			{
				object[] customAttributes = member[0].GetCustomAttributes(false);
				for (int i = 0; i < (int)customAttributes.Length; i++)
				{
					object obj = customAttributes[i];
					if (obj is DisplayAttribute)
					{
						return ((DisplayAttribute)obj).get_Description();
					}
				}
			}
			return val.ToString();
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		public override string GetTitle()
		{
			return this.GetEnumTitle((Enum)(object)this.Value);
		}

		public override void Init(IApi api)
		{
			this.InitMetaData();
		}

		private void InitMetaData()
		{
			if (SelectField<T>.IsInitialized)
			{
				return;
			}
			lock (SelectField<T>.Mutex)
			{
				if (!SelectField<T>.IsInitialized)
				{
					foreach (object value in Enum.GetValues(typeof(T)))
					{
						SelectField<T>._items.Add(new SelectFieldItem()
						{
							Title = this.GetEnumTitle((Enum)value),
							Value = (Enum)value
						});
					}
					SelectField<T>.IsInitialized = true;
				}
			}
		}

		public static bool operator ==(SelectField<T> field1, SelectField<T> field2)
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

		public static bool operator !=(SelectField<T> field1, SelectField<T> field2)
		{
			return !(field1 == field2);
		}
	}
}