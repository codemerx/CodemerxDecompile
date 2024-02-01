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

		private FlagsWriter.EnumNameCollection<MethodAttributes> methodAttributeFlags = new FlagsWriter.EnumNameCollection<MethodAttributes>()
		{
			{ 32, "final" },
			{ 128, "hidebysig" },
			{ 0x800, "specialname" },
			{ 0x2000, null },
			{ 8, "export" },
			{ 0x1000, "rtspecialname" },
			{ 0x8000, "reqsecobj" },
			{ 0x100, "newslot" },
			{ 0x200, "strict" },
			{ 0x400, "abstract" },
			{ 64, "virtual" },
			{ 16, "static" },
			{ 0x4000, null }
		};

		private FlagsWriter.EnumNameCollection<MethodAttributes> methodVisibility = new FlagsWriter.EnumNameCollection<MethodAttributes>()
		{
			{ 1, "private" },
			{ 2, "famandassem" },
			{ 3, "assembly" },
			{ 4, "family" },
			{ 5, "famorassem" },
			{ 6, "public" }
		};

		private FlagsWriter.EnumNameCollection<MethodCallingConvention> callingConvention = new FlagsWriter.EnumNameCollection<MethodCallingConvention>()
		{
			{ 1, "unmanaged cdecl" },
			{ 2, "unmanaged stdcall" },
			{ 3, "unmanaged thiscall" },
			{ 4, "unmanaged fastcall" },
			{ 5, "vararg" },
			{ 16, null }
		};

		private FlagsWriter.EnumNameCollection<MethodImplAttributes> methodCodeType = new FlagsWriter.EnumNameCollection<MethodImplAttributes>()
		{
			{ 0, "cil" },
			{ 1, "native" },
			{ 2, "optil" },
			{ 3, "runtime" }
		};

		private FlagsWriter.EnumNameCollection<MethodImplAttributes> methodImpl = new FlagsWriter.EnumNameCollection<MethodImplAttributes>()
		{
			{ 32, "synchronized" },
			{ 8, "noinlining" },
			{ 64, "nooptimization" },
			{ 128, "preservesig" },
			{ 0x1000, "internalcall" },
			{ 16, "forwardref" },
			{ 0x100, "aggressiveinlining" }
		};

		private FlagsWriter.EnumNameCollection<FieldAttributes> fieldVisibility = new FlagsWriter.EnumNameCollection<FieldAttributes>()
		{
			{ 1, "private" },
			{ 2, "famandassem" },
			{ 3, "assembly" },
			{ 4, "family" },
			{ 5, "famorassem" },
			{ 6, "public" }
		};

		private FlagsWriter.EnumNameCollection<FieldAttributes> fieldAttributes = new FlagsWriter.EnumNameCollection<FieldAttributes>()
		{
			{ 16, "static" },
			{ 64, "literal" },
			{ 32, "initonly" },
			{ 0x200, "specialname" },
			{ 0x400, "rtspecialname" },
			{ 128, "notserialized" }
		};

		private FlagsWriter.EnumNameCollection<PropertyAttributes> propertyAttributes = new FlagsWriter.EnumNameCollection<PropertyAttributes>()
		{
			{ 0x200, "specialname" },
			{ 0x400, "rtspecialname" },
			{ 0x1000, "hasdefault" }
		};

		private FlagsWriter.EnumNameCollection<EventAttributes> eventAttributes = new FlagsWriter.EnumNameCollection<EventAttributes>()
		{
			{ 0x200, "specialname" },
			{ 0x400, "rtspecialname" }
		};

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeVisibility = new FlagsWriter.EnumNameCollection<TypeAttributes>()
		{
			{ 1, "public" },
			{ 0, "private" },
			{ 2, "nested public" },
			{ 3, "nested private" },
			{ 5, "nested assembly" },
			{ 4, "nested family" },
			{ 6, "nested famandassem" },
			{ 7, "nested famorassem" }
		};

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeLayout = new FlagsWriter.EnumNameCollection<TypeAttributes>()
		{
			{ 0, "auto" },
			{ 8, "sequential" },
			{ 16, "explicit" }
		};

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeStringFormat = new FlagsWriter.EnumNameCollection<TypeAttributes>()
		{
			{ 0x20000, "auto" },
			{ 0, "ansi" },
			{ 0x10000, "unicode" }
		};

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeAttributes = new FlagsWriter.EnumNameCollection<TypeAttributes>()
		{
			{ 128, "abstract" },
			{ 0x100, "sealed" },
			{ 0x400, "specialname" },
			{ 0x1000, "import" },
			{ 0x2000, "serializable" },
			{ 0x100000, "beforefieldinit" },
			{ 0x40000, null }
		};

		internal FlagsWriter(BaseLanguageWriter languageWriter)
		{
			this.languageWriter = languageWriter;
		}

		private void WriteEnum<T>(T enumValue, FlagsWriter.EnumNameCollection<T> enumNames)
		where T : struct
		{
			long num = Convert.ToInt64(enumValue);
			foreach (KeyValuePair<long, string> enumName in enumNames)
			{
				if (enumName.Key != num)
				{
					continue;
				}
				if (enumName.Value != null)
				{
					this.languageWriter.WriteKeyword(enumName.Value);
					this.languageWriter.WriteSpace();
				}
				return;
			}
			if (num != 0)
			{
				this.languageWriter.WriteKeyword(String.Format("flag({0:x4})", num));
				this.languageWriter.WriteSpace();
			}
		}

		internal void WriteEventFlags(EventDefinition @event)
		{
			this.WriteFlags<EventAttributes>(@event.get_Attributes(), this.eventAttributes);
		}

		internal void WriteFieldFlags(FieldDefinition field)
		{
			this.WriteFlags<FieldAttributes>(field.get_Attributes() & 0x6ef8, this.fieldAttributes);
		}

		internal void WriteFieldVisibility(FieldDefinition field)
		{
			this.WriteEnum<FieldAttributes>(field.get_Attributes() & 7, this.fieldVisibility);
		}

		private void WriteFlags<T>(T flags, FlagsWriter.EnumNameCollection<T> flagNames)
		where T : struct
		{
			long num = Convert.ToInt64(flags);
			long key = (long)0;
			foreach (KeyValuePair<long, string> flagName in flagNames)
			{
				key |= flagName.Key;
				if ((num & flagName.Key) == 0 || flagName.Value == null)
				{
					continue;
				}
				this.languageWriter.WriteKeyword(flagName.Value);
				this.languageWriter.WriteSpace();
			}
			if ((num & ~key) != 0)
			{
				this.languageWriter.WriteKeyword(String.Format("flag({0:x4}) ", num & ~key));
			}
		}

		internal void WriteMethodCallingConvention(MethodDefinition method)
		{
			this.WriteEnum<MethodCallingConvention>(method.get_CallingConvention() & 31, this.callingConvention);
		}

		internal void WriteMethodCallType(MethodDefinition method)
		{
			this.WriteEnum<MethodImplAttributes>(method.get_ImplAttributes() & 3, this.methodCodeType);
		}

		internal void WriteMethodFlags(MethodDefinition method)
		{
			this.WriteFlags<MethodAttributes>(method.get_Attributes() & 0xfff8, this.methodAttributeFlags);
		}

		internal void WriteMethodImplementationAttribute(MethodDefinition method)
		{
			this.WriteFlags<MethodImplAttributes>(method.get_ImplAttributes() & 0xfff8, this.methodImpl);
		}

		internal void WriteMethodVisibility(MethodDefinition method)
		{
			this.WriteEnum<MethodAttributes>(method.get_Attributes() & 7, this.methodVisibility);
		}

		internal void WritePropertyFlags(PropertyDefinition property)
		{
			this.WriteFlags<PropertyAttributes>(property.get_Attributes(), this.propertyAttributes);
		}

		internal void WriteTypeAttributes(TypeDefinition type)
		{
			this.WriteFlags<TypeAttributes>(type.get_Attributes() & -196672, this.typeAttributes);
		}

		internal void WriteTypeLayoutFlags(TypeDefinition type)
		{
			this.WriteEnum<TypeAttributes>(type.get_Attributes() & 24, this.typeLayout);
		}

		internal void WriteTypeStringFormat(TypeDefinition type)
		{
			this.WriteEnum<TypeAttributes>(type.get_Attributes() & 0x30000, this.typeStringFormat);
		}

		internal void WriteTypeVisibility(TypeDefinition type)
		{
			this.WriteEnum<TypeAttributes>(type.get_Attributes() & 7, this.typeVisibility);
		}

		private sealed class EnumNameCollection<T> : IEnumerable<KeyValuePair<long, string>>, IEnumerable
		where T : struct
		{
			private List<KeyValuePair<long, string>> names;

			public EnumNameCollection()
			{
			}

			public void Add(T flag, string name)
			{
				this.names.Add(new KeyValuePair<long, string>(Convert.ToInt64(flag), name));
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