using Piranha;
using Piranha.Extend;
using System;
using System.Collections;
using System.Collections.Generic;
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
				return Type.GetTypeFromHandle(// 
				// Current member / type: System.Type Piranha.Extend.Fields.SelectField`1::EnumType()
				// Exception in: System.Type EnumType()
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override string EnumValue
		{
			get
			{
				return this.get_Value().ToString();
			}
			set
			{
				this.set_Value((T)Enum.Parse(Type.GetTypeFromHandle(// 
				// Current member / type: System.String Piranha.Extend.Fields.SelectField`1::EnumValue()
				// Exception in: System.String EnumValue()
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


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
			return;
		}

		public SelectField()
		{
			base();
			return;
		}

		public override bool Equals(object obj)
		{
			V_0 = obj as SelectField<T>;
			if (V_0 == null)
			{
				return false;
			}
			return this.Equals(V_0);
		}

		public virtual bool Equals(SelectField<T> obj)
		{
			if (SelectField<T>.op_Equality(obj, null))
			{
				return false;
			}
			return EqualityComparer<T>.get_Default().Equals(this.get_Value(), obj.get_Value());
		}

		private string GetEnumTitle(Enum val)
		{
			V_0 = Type.GetTypeFromHandle(// 
			// Current member / type: System.String Piranha.Extend.Fields.SelectField`1::GetEnumTitle(System.Enum)
			// Exception in: System.String GetEnumTitle(System.Enum)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public override int GetHashCode()
		{
			return this.get_Value().GetHashCode();
		}

		public override string GetTitle()
		{
			return this.GetEnumTitle((Enum)(object)this.get_Value());
		}

		public override void Init(IApi api)
		{
			this.InitMetaData();
			return;
		}

		private void InitMetaData()
		{
			if (SelectField<T>.IsInitialized)
			{
				return;
			}
			V_0 = SelectField<T>.Mutex;
			V_1 = false;
			try
			{
				Monitor.Enter(V_0, ref V_1);
				if (!SelectField<T>.IsInitialized)
				{
					V_2 = Enum.GetValues(Type.GetTypeFromHandle(// 
					// Current member / type: System.Void Piranha.Extend.Fields.SelectField`1::InitMetaData()
					// Exception in: System.Void InitMetaData()
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


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
			return !SelectField<T>.op_Equality(field1, field2);
		}
	}
}