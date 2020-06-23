using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.IL
{
	internal static class ILHelpers
	{
		private readonly static HashSet<string> ilKeywords;

		static ILHelpers()
		{
			ILHelpers.ilKeywords = ILHelpers.BuildKeywordList(new String[] { "abstract", "algorithm", "alignment", "ansi", "any", "arglist", "array", "as", "assembly", "assert", "at", "auto", "autochar", "beforefieldinit", "blob", "blob_object", "bool", "brnull", "brnull.s", "brzero", "brzero.s", "bstr", "bytearray", "byvalstr", "callmostderived", "carray", "catch", "cdecl", "cf", "char", "cil", "class", "clsid", "const", "currency", "custom", "date", "decimal", "default", "demand", "deny", "endmac", "enum", "error", "explicit", "extends", "extern", "false", "famandassem", "family", "famorassem", "fastcall", "fault", "field", "filetime", "filter", "final", "finally", "fixed", "float", "float32", "float64", "forwardref", "fromunmanaged", "handler", "hidebysig", "hresult", "idispatch", "il", "illegal", "implements", "implicitcom", "implicitres", "import", "in", "inheritcheck", "init", "initonly", "instance", "int", "int16", "int32", "int64", "int8", "interface", "internalcall", "iunknown", "lasterr", "lcid", "linkcheck", "literal", "localloc", "lpstr", "lpstruct", "lptstr", "lpvoid", "lpwstr", "managed", "marshal", "method", "modopt", "modreq", "native", "nested", "newslot", "noappdomain", "noinlining", "nomachine", "nomangle", "nometadata", "noncasdemand", "noncasinheritance", "noncaslinkdemand", "noprocess", "not", "not_in_gc_heap", "notremotable", "notserialized", "null", "nullref", "object", "objectref", "opt", "optil", "out", "permitonly", "pinned", "pinvokeimpl", "prefix1", "prefix2", "prefix3", "prefix4", "prefix5", "prefix6", "prefix7", "prefixref", "prejitdeny", "prejitgrant", "preservesig", "private", "privatescope", "protected", "public", "record", "refany", "reqmin", "reqopt", "reqrefuse", "reqsecobj", "request", "retval", "rtspecialname", "runtime", "safearray", "sealed", "sequential", "serializable", "special", "specialname", "static", "stdcall", "storage", "stored_object", "stream", "streamed_object", "string", "struct", "synchronized", "syschar", "sysstring", "tbstr", "thiscall", "tls", "to", "true", "typedref", "unicode", "unmanaged", "unmanagedexp", "unsigned", "unused", "userdefined", "value", "valuetype", "vararg", "variant", "vector", "virtual", "void", "wchar", "winapi", "with", "wrapper", "property", "type", "flags", "callconv", "strict", "aggressiveinlining" });
		}

		internal static HashSet<string> BuildKeywordList(params string[] keywords)
		{
			HashSet<string> strs = new HashSet<string>(keywords);
			FieldInfo[] fields = typeof(OpCodes).GetFields();
			for (int i = 0; i < (int)fields.Length; i++)
			{
				OpCode value = (OpCode)fields[i].GetValue(null);
				strs.Add(value.Name);
			}
			return strs;
		}

		internal static MemberMapping CreateCodeMapping(MethodDefinition member, CodeMappings codeMappings)
		{
			if (member == null || !member.HasBody)
			{
				return null;
			}
			if (codeMappings == null)
			{
				return null;
			}
			MemberMapping memberMapping = null;
			if (codeMappings.FullName == member.DeclaringType.FullName)
			{
				List<MemberMapping> mapping = codeMappings.Mapping;
				if (mapping.Find((MemberMapping map) => map.MetadataToken == member.MetadataToken.ToInt32()) == null)
				{
					memberMapping = new MemberMapping()
					{
						MetadataToken = (uint)member.MetadataToken.ToInt32(),
						MemberReference = member.DeclaringType.Resolve(),
						MemberCodeMappings = new List<SourceCodeMapping>(),
						CodeSize = member.Body.CodeSize
					};
					mapping.Add(memberMapping);
				}
			}
			return memberMapping;
		}

		internal static string Escape(string identifier)
		{
			if (ILHelpers.IsValidIdentifier(identifier) && !ILHelpers.ilKeywords.Contains(identifier))
			{
				return identifier;
			}
			return String.Concat("'", BaseLanguageWriter.ConvertString(identifier).Replace("'", "\\'"), "'");
		}

		internal static bool IsValidIdentifier(string identifier)
		{
			if (String.IsNullOrEmpty(identifier))
			{
				return false;
			}
			if (!Char.IsLetter(identifier[0]) && !ILHelpers.IsValidIdentifierCharacter(identifier[0]))
			{
				if (identifier == ".ctor")
				{
					return true;
				}
				return identifier == ".cctor";
			}
			for (int i = 1; i < identifier.Length; i++)
			{
				if (!Char.IsLetterOrDigit(identifier[i]) && !ILHelpers.IsValidIdentifierCharacter(identifier[i]) && identifier[i] != '.')
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsValidIdentifierCharacter(char c)
		{
			if (c == '\u005F' || c == '$' || c == '@' || c == '?')
			{
				return true;
			}
			return c == '\u0060';
		}

		internal static string PrimitiveTypeName(string typeName)
		{
			if (typeName != null)
			{
				if (typeName == "System.SByte")
				{
					return "int8";
				}
				if (typeName == "System.Int16")
				{
					return "int16";
				}
				if (typeName == "System.Int32")
				{
					return "int32";
				}
				if (typeName == "System.Int64")
				{
					return "int64";
				}
				if (typeName == "System.Byte")
				{
					return "uint8";
				}
				if (typeName == "System.UInt16")
				{
					return "uint16";
				}
				if (typeName == "System.UInt32")
				{
					return "uint32";
				}
				if (typeName == "System.UInt64")
				{
					return "uint64";
				}
				if (typeName == "System.Single")
				{
					return "float32";
				}
				if (typeName == "System.Double")
				{
					return "float64";
				}
				if (typeName == "System.Void")
				{
					return "void";
				}
				if (typeName == "System.Boolean")
				{
					return "bool";
				}
				if (typeName == "System.String")
				{
					return "string";
				}
				if (typeName == "System.Char")
				{
					return "char";
				}
				if (typeName == "System.Object")
				{
					return "object";
				}
				if (typeName == "System.IntPtr")
				{
					return "native int";
				}
			}
			return null;
		}

		internal static string ToInvariantCultureString(object value)
		{
			IConvertible convertible = value as IConvertible;
			if (convertible == null)
			{
				return value.ToString();
			}
			return convertible.ToString(CultureInfo.InvariantCulture);
		}
	}
}