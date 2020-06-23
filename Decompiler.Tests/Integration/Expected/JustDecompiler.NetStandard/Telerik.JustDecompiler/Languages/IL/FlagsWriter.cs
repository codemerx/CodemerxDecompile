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
			{ MethodAttributes.Final, "final" },
			{ MethodAttributes.HideBySig, "hidebysig" },
			{ MethodAttributes.SpecialName, "specialname" },
			{ MethodAttributes.PInvokeImpl, null },
			{ MethodAttributes.UnmanagedExport, "export" },
			{ MethodAttributes.RTSpecialName, "rtspecialname" },
			{ MethodAttributes.RequireSecObject, "reqsecobj" },
			{ MethodAttributes.VtableLayoutMask, "newslot" },
			{ MethodAttributes.CheckAccessOnOverride, "strict" },
			{ MethodAttributes.Abstract, "abstract" },
			{ MethodAttributes.Virtual, "virtual" },
			{ MethodAttributes.Static, "static" },
			{ MethodAttributes.HasSecurity, null }
		};

		private FlagsWriter.EnumNameCollection<MethodAttributes> methodVisibility = new FlagsWriter.EnumNameCollection<MethodAttributes>()
		{
			{ MethodAttributes.Private, "private" },
			{ MethodAttributes.FamANDAssem, "famandassem" },
			{ MethodAttributes.Assembly, "assembly" },
			{ MethodAttributes.Family, "family" },
			{ MethodAttributes.FamORAssem, "famorassem" },
			{ MethodAttributes.Public, "public" }
		};

		private FlagsWriter.EnumNameCollection<MethodCallingConvention> callingConvention = new FlagsWriter.EnumNameCollection<MethodCallingConvention>()
		{
			{ MethodCallingConvention.C, "unmanaged cdecl" },
			{ MethodCallingConvention.StdCall, "unmanaged stdcall" },
			{ MethodCallingConvention.ThisCall, "unmanaged thiscall" },
			{ MethodCallingConvention.FastCall, "unmanaged fastcall" },
			{ MethodCallingConvention.VarArg, "vararg" },
			{ MethodCallingConvention.Generic, null }
		};

		private FlagsWriter.EnumNameCollection<MethodImplAttributes> methodCodeType = new FlagsWriter.EnumNameCollection<MethodImplAttributes>()
		{
			{ MethodImplAttributes.IL, "cil" },
			{ MethodImplAttributes.Native, "native" },
			{ MethodImplAttributes.OPTIL, "optil" },
			{ MethodImplAttributes.CodeTypeMask, "runtime" }
		};

		private FlagsWriter.EnumNameCollection<MethodImplAttributes> methodImpl = new FlagsWriter.EnumNameCollection<MethodImplAttributes>()
		{
			{ MethodImplAttributes.Synchronized, "synchronized" },
			{ MethodImplAttributes.NoInlining, "noinlining" },
			{ MethodImplAttributes.NoOptimization, "nooptimization" },
			{ MethodImplAttributes.PreserveSig, "preservesig" },
			{ MethodImplAttributes.InternalCall, "internalcall" },
			{ MethodImplAttributes.ForwardRef, "forwardref" },
			{ MethodImplAttributes.AggressiveInlining, "aggressiveinlining" }
		};

		private FlagsWriter.EnumNameCollection<FieldAttributes> fieldVisibility = new FlagsWriter.EnumNameCollection<FieldAttributes>()
		{
			{ FieldAttributes.Private, "private" },
			{ FieldAttributes.FamANDAssem, "famandassem" },
			{ FieldAttributes.Assembly, "assembly" },
			{ FieldAttributes.Family, "family" },
			{ FieldAttributes.FamORAssem, "famorassem" },
			{ FieldAttributes.Public, "public" }
		};

		private FlagsWriter.EnumNameCollection<FieldAttributes> fieldAttributes = new FlagsWriter.EnumNameCollection<FieldAttributes>()
		{
			{ FieldAttributes.Static, "static" },
			{ FieldAttributes.Literal, "literal" },
			{ FieldAttributes.InitOnly, "initonly" },
			{ FieldAttributes.SpecialName, "specialname" },
			{ FieldAttributes.RTSpecialName, "rtspecialname" },
			{ FieldAttributes.NotSerialized, "notserialized" }
		};

		private FlagsWriter.EnumNameCollection<PropertyAttributes> propertyAttributes = new FlagsWriter.EnumNameCollection<PropertyAttributes>()
		{
			{ PropertyAttributes.SpecialName, "specialname" },
			{ PropertyAttributes.RTSpecialName, "rtspecialname" },
			{ PropertyAttributes.HasDefault, "hasdefault" }
		};

		private FlagsWriter.EnumNameCollection<EventAttributes> eventAttributes = new FlagsWriter.EnumNameCollection<EventAttributes>()
		{
			{ EventAttributes.SpecialName, "specialname" },
			{ EventAttributes.RTSpecialName, "rtspecialname" }
		};

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeVisibility = new FlagsWriter.EnumNameCollection<TypeAttributes>()
		{
			{ TypeAttributes.Public, "public" },
			{ TypeAttributes.NotPublic, "private" },
			{ TypeAttributes.NestedPublic, "nested public" },
			{ TypeAttributes.NestedPrivate, "nested private" },
			{ TypeAttributes.NestedAssembly, "nested assembly" },
			{ TypeAttributes.NestedFamily, "nested family" },
			{ TypeAttributes.NestedFamANDAssem, "nested famandassem" },
			{ TypeAttributes.VisibilityMask, "nested famorassem" }
		};

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeLayout = new FlagsWriter.EnumNameCollection<TypeAttributes>()
		{
			{ TypeAttributes.NotPublic, "auto" },
			{ TypeAttributes.SequentialLayout, "sequential" },
			{ TypeAttributes.ExplicitLayout, "explicit" }
		};

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeStringFormat = new FlagsWriter.EnumNameCollection<TypeAttributes>()
		{
			{ TypeAttributes.AutoClass, "auto" },
			{ TypeAttributes.NotPublic, "ansi" },
			{ TypeAttributes.UnicodeClass, "unicode" }
		};

		private FlagsWriter.EnumNameCollection<TypeAttributes> typeAttributes = new FlagsWriter.EnumNameCollection<TypeAttributes>()
		{
			{ TypeAttributes.Abstract, "abstract" },
			{ TypeAttributes.Sealed, "sealed" },
			{ TypeAttributes.SpecialName, "specialname" },
			{ TypeAttributes.Import, "import" },
			{ TypeAttributes.Serializable, "serializable" },
			{ TypeAttributes.BeforeFieldInit, "beforefieldinit" },
			{ TypeAttributes.HasSecurity, null }
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
			this.WriteFlags<EventAttributes>(@event.Attributes, this.eventAttributes);
		}

		internal void WriteFieldFlags(FieldDefinition field)
		{
			this.WriteFlags<FieldAttributes>(field.Attributes & (FieldAttributes.Static | FieldAttributes.InitOnly | FieldAttributes.Literal | FieldAttributes.NotSerialized | FieldAttributes.SpecialName | FieldAttributes.PInvokeImpl | FieldAttributes.RTSpecialName), this.fieldAttributes);
		}

		internal void WriteFieldVisibility(FieldDefinition field)
		{
			this.WriteEnum<FieldAttributes>(field.Attributes & FieldAttributes.FieldAccessMask, this.fieldVisibility);
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
			this.WriteEnum<MethodCallingConvention>(method.CallingConvention & (MethodCallingConvention.C | MethodCallingConvention.StdCall | MethodCallingConvention.ThisCall | MethodCallingConvention.FastCall | MethodCallingConvention.VarArg | MethodCallingConvention.Generic), this.callingConvention);
		}

		internal void WriteMethodCallType(MethodDefinition method)
		{
			this.WriteEnum<MethodImplAttributes>(method.ImplAttributes & MethodImplAttributes.CodeTypeMask, this.methodCodeType);
		}

		internal void WriteMethodFlags(MethodDefinition method)
		{
			this.WriteFlags<MethodAttributes>(method.Attributes & (MethodAttributes.Static | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.NewSlot | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Abstract | MethodAttributes.SpecialName | MethodAttributes.PInvokeImpl | MethodAttributes.UnmanagedExport | MethodAttributes.RTSpecialName | MethodAttributes.HasSecurity | MethodAttributes.RequireSecObject), this.methodAttributeFlags);
		}

		internal void WriteMethodImplementationAttribute(MethodDefinition method)
		{
			this.WriteFlags<MethodImplAttributes>(method.ImplAttributes & (MethodImplAttributes.ForwardRef | MethodImplAttributes.PreserveSig | MethodImplAttributes.InternalCall | MethodImplAttributes.Synchronized | MethodImplAttributes.NoOptimization | MethodImplAttributes.NoInlining | MethodImplAttributes.AggressiveInlining), this.methodImpl);
		}

		internal void WriteMethodVisibility(MethodDefinition method)
		{
			this.WriteEnum<MethodAttributes>(method.Attributes & MethodAttributes.MemberAccessMask, this.methodVisibility);
		}

		internal void WritePropertyFlags(PropertyDefinition property)
		{
			this.WriteFlags<PropertyAttributes>(property.Attributes, this.propertyAttributes);
		}

		internal void WriteTypeAttributes(TypeDefinition type)
		{
			this.WriteFlags<TypeAttributes>(type.Attributes & (TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.SpecialName | TypeAttributes.Import | TypeAttributes.Serializable | TypeAttributes.WindowsRuntime | TypeAttributes.BeforeFieldInit | TypeAttributes.RTSpecialName | TypeAttributes.HasSecurity | TypeAttributes.Forwarder), this.typeAttributes);
		}

		internal void WriteTypeLayoutFlags(TypeDefinition type)
		{
			this.WriteEnum<TypeAttributes>(type.Attributes & TypeAttributes.LayoutMask, this.typeLayout);
		}

		internal void WriteTypeStringFormat(TypeDefinition type)
		{
			this.WriteEnum<TypeAttributes>(type.Attributes & TypeAttributes.StringFormatMask, this.typeStringFormat);
		}

		internal void WriteTypeVisibility(TypeDefinition type)
		{
			this.WriteEnum<TypeAttributes>(type.Attributes & TypeAttributes.VisibilityMask, this.typeVisibility);
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