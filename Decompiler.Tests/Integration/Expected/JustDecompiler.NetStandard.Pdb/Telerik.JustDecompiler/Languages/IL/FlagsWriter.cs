using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.IL
{
	internal class FlagsWriter
	{
		private BaseLanguageWriter languageWriter;

		private FlagsWriter.EnumNameCollection<MethodAttributes> methodAttributeFlags;

		private FlagsWriter.EnumNameCollection<MethodAttributes> methodVisibility;

		private FlagsWriter.EnumNameCollection<MethodCallingConvention> callingConvention;

		private FlagsWriter.EnumNameCollection<MethodImplAttributes> methodCodeType;

		private FlagsWriter.EnumNameCollection<MethodImplAttributes> methodImpl;

		private FlagsWriter.EnumNameCollection<FieldAttributes> fieldVisibility;

		private FlagsWriter.EnumNameCollection<FieldAttributes> fieldAttributes;

		private FlagsWriter.EnumNameCollection<PropertyAttributes> propertyAttributes;

		private FlagsWriter.EnumNameCollection<EventAttributes> eventAttributes;

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeVisibility;

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeLayout;

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeStringFormat;

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeAttributes;

		internal FlagsWriter(BaseLanguageWriter languageWriter)
		{
			stackVariable1 = new FlagsWriter.EnumNameCollection<MethodAttributes>();
			stackVariable1.Add(32, "final");
			stackVariable1.Add(128, "hidebysig");
			stackVariable1.Add(0x800, "specialname");
			stackVariable1.Add(0x2000, null);
			stackVariable1.Add(8, "export");
			stackVariable1.Add(0x1000, "rtspecialname");
			stackVariable1.Add(0x8000, "reqsecobj");
			stackVariable1.Add(0x100, "newslot");
			stackVariable1.Add(0x200, "strict");
			stackVariable1.Add(0x400, "abstract");
			stackVariable1.Add(64, "virtual");
			stackVariable1.Add(16, "static");
			stackVariable1.Add(0x4000, null);
			this.methodAttributeFlags = stackVariable1;
			stackVariable29 = new FlagsWriter.EnumNameCollection<MethodAttributes>();
			stackVariable29.Add(1, "private");
			stackVariable29.Add(2, "famandassem");
			stackVariable29.Add(3, "assembly");
			stackVariable29.Add(4, "family");
			stackVariable29.Add(5, "famorassem");
			stackVariable29.Add(6, "public");
			this.methodVisibility = stackVariable29;
			stackVariable43 = new FlagsWriter.EnumNameCollection<MethodCallingConvention>();
			stackVariable43.Add(1, "unmanaged cdecl");
			stackVariable43.Add(2, "unmanaged stdcall");
			stackVariable43.Add(3, "unmanaged thiscall");
			stackVariable43.Add(4, "unmanaged fastcall");
			stackVariable43.Add(5, "vararg");
			stackVariable43.Add(16, null);
			this.callingConvention = stackVariable43;
			stackVariable57 = new FlagsWriter.EnumNameCollection<MethodImplAttributes>();
			stackVariable57.Add(0, "cil");
			stackVariable57.Add(1, "native");
			stackVariable57.Add(2, "optil");
			stackVariable57.Add(3, "runtime");
			this.methodCodeType = stackVariable57;
			stackVariable67 = new FlagsWriter.EnumNameCollection<MethodImplAttributes>();
			stackVariable67.Add(32, "synchronized");
			stackVariable67.Add(8, "noinlining");
			stackVariable67.Add(64, "nooptimization");
			stackVariable67.Add(128, "preservesig");
			stackVariable67.Add(0x1000, "internalcall");
			stackVariable67.Add(16, "forwardref");
			stackVariable67.Add(0x100, "aggressiveinlining");
			this.methodImpl = stackVariable67;
			stackVariable83 = new FlagsWriter.EnumNameCollection<FieldAttributes>();
			stackVariable83.Add(1, "private");
			stackVariable83.Add(2, "famandassem");
			stackVariable83.Add(3, "assembly");
			stackVariable83.Add(4, "family");
			stackVariable83.Add(5, "famorassem");
			stackVariable83.Add(6, "public");
			this.fieldVisibility = stackVariable83;
			stackVariable97 = new FlagsWriter.EnumNameCollection<FieldAttributes>();
			stackVariable97.Add(16, "static");
			stackVariable97.Add(64, "literal");
			stackVariable97.Add(32, "initonly");
			stackVariable97.Add(0x200, "specialname");
			stackVariable97.Add(0x400, "rtspecialname");
			stackVariable97.Add(128, "notserialized");
			this.fieldAttributes = stackVariable97;
			stackVariable111 = new FlagsWriter.EnumNameCollection<PropertyAttributes>();
			stackVariable111.Add(0x200, "specialname");
			stackVariable111.Add(0x400, "rtspecialname");
			stackVariable111.Add(0x1000, "hasdefault");
			this.propertyAttributes = stackVariable111;
			stackVariable119 = new FlagsWriter.EnumNameCollection<EventAttributes>();
			stackVariable119.Add(0x200, "specialname");
			stackVariable119.Add(0x400, "rtspecialname");
			this.eventAttributes = stackVariable119;
			stackVariable125 = new FlagsWriter.EnumNameCollection<TypeAttributes>();
			stackVariable125.Add(1, "public");
			stackVariable125.Add(0, "private");
			stackVariable125.Add(2, "nested public");
			stackVariable125.Add(3, "nested private");
			stackVariable125.Add(5, "nested assembly");
			stackVariable125.Add(4, "nested family");
			stackVariable125.Add(6, "nested famandassem");
			stackVariable125.Add(7, "nested famorassem");
			this.typeVisibility = stackVariable125;
			stackVariable143 = new FlagsWriter.EnumNameCollection<TypeAttributes>();
			stackVariable143.Add(0, "auto");
			stackVariable143.Add(8, "sequential");
			stackVariable143.Add(16, "explicit");
			this.typeLayout = stackVariable143;
			stackVariable151 = new FlagsWriter.EnumNameCollection<TypeAttributes>();
			stackVariable151.Add(0x20000, "auto");
			stackVariable151.Add(0, "ansi");
			stackVariable151.Add(0x10000, "unicode");
			this.typeStringFormat = stackVariable151;
			stackVariable159 = new FlagsWriter.EnumNameCollection<TypeAttributes>();
			stackVariable159.Add(128, "abstract");
			stackVariable159.Add(0x100, "sealed");
			stackVariable159.Add(0x400, "specialname");
			stackVariable159.Add(0x1000, "import");
			stackVariable159.Add(0x2000, "serializable");
			stackVariable159.Add(0x100000, "beforefieldinit");
			stackVariable159.Add(0x40000, null);
			this.typeAttributes = stackVariable159;
			base();
			this.languageWriter = languageWriter;
			return;
		}

		private void WriteEnum<T>(T enumValue, FlagsWriter.EnumNameCollection<T> enumNames)
		where T : struct
		{
			V_0 = Convert.ToInt64(enumValue);
			V_1 = enumNames.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_Key() != V_0)
					{
						continue;
					}
					if (V_2.get_Value() != null)
					{
						this.languageWriter.WriteKeyword(V_2.get_Value());
						this.languageWriter.WriteSpace();
					}
					goto Label0;
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			if (V_0 != 0)
			{
				this.languageWriter.WriteKeyword(String.Format("flag({0:x4})", V_0));
				this.languageWriter.WriteSpace();
			}
		Label0:
			return;
		}

		internal void WriteEventFlags(EventDefinition event)
		{
			this.WriteFlags<EventAttributes>(event.get_Attributes(), this.eventAttributes);
			return;
		}

		internal void WriteFieldFlags(FieldDefinition field)
		{
			this.WriteFlags<FieldAttributes>(field.get_Attributes() & 0x6ef8, this.fieldAttributes);
			return;
		}

		internal void WriteFieldVisibility(FieldDefinition field)
		{
			this.WriteEnum<FieldAttributes>(field.get_Attributes() & 7, this.fieldVisibility);
			return;
		}

		private void WriteFlags<T>(T flags, FlagsWriter.EnumNameCollection<T> flagNames)
		where T : struct
		{
			V_0 = Convert.ToInt64(flags);
			V_1 = (long)0;
			V_2 = flagNames.GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					V_1 = V_1 | V_3.get_Key();
					if (V_0 & V_3.get_Key() == 0 || V_3.get_Value() == null)
					{
						continue;
					}
					this.languageWriter.WriteKeyword(V_3.get_Value());
					this.languageWriter.WriteSpace();
				}
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			if (V_0 & ~V_1 != 0)
			{
				this.languageWriter.WriteKeyword(String.Format("flag({0:x4}) ", V_0 & ~V_1));
			}
			return;
		}

		internal void WriteMethodCallingConvention(MethodDefinition method)
		{
			this.WriteEnum<MethodCallingConvention>(method.get_CallingConvention() & 31, this.callingConvention);
			return;
		}

		internal void WriteMethodCallType(MethodDefinition method)
		{
			this.WriteEnum<MethodImplAttributes>(method.get_ImplAttributes() & 3, this.methodCodeType);
			return;
		}

		internal void WriteMethodFlags(MethodDefinition method)
		{
			this.WriteFlags<MethodAttributes>(method.get_Attributes() & 0xfff8, this.methodAttributeFlags);
			return;
		}

		internal void WriteMethodImplementationAttribute(MethodDefinition method)
		{
			this.WriteFlags<MethodImplAttributes>(method.get_ImplAttributes() & 0xfff8, this.methodImpl);
			return;
		}

		internal void WriteMethodVisibility(MethodDefinition method)
		{
			this.WriteEnum<MethodAttributes>(method.get_Attributes() & 7, this.methodVisibility);
			return;
		}

		internal void WritePropertyFlags(PropertyDefinition property)
		{
			this.WriteFlags<PropertyAttributes>(property.get_Attributes(), this.propertyAttributes);
			return;
		}

		internal void WriteTypeAttributes(TypeDefinition type)
		{
			this.WriteFlags<TypeAttributes>(type.get_Attributes() & -196672, this.typeAttributes);
			return;
		}

		internal void WriteTypeLayoutFlags(TypeDefinition type)
		{
			this.WriteEnum<TypeAttributes>(type.get_Attributes() & 24, this.typeLayout);
			return;
		}

		internal void WriteTypeStringFormat(TypeDefinition type)
		{
			this.WriteEnum<TypeAttributes>(type.get_Attributes() & 0x30000, this.typeStringFormat);
			return;
		}

		internal void WriteTypeVisibility(TypeDefinition type)
		{
			this.WriteEnum<TypeAttributes>(type.get_Attributes() & 7, this.typeVisibility);
			return;
		}

		private sealed class EnumNameCollection<T> : IEnumerable<KeyValuePair<long, string>>, IEnumerable
		where T : struct
		{
			private List<KeyValuePair<long, string>> names;

			public EnumNameCollection()
			{
				this.names = new List<KeyValuePair<long, string>>();
				base();
				return;
			}

			public void Add(T flag, string name)
			{
				this.names.Add(new KeyValuePair<long, string>(Convert.ToInt64(flag), name));
				return;
			}

			public IEnumerator<KeyValuePair<long, string>> GetEnumerator()
			{
				return this.names.GetEnumerator();
			}

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.names.GetEnumerator();
			}
		}
	}
}