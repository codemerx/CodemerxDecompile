using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Languages.IL
{
	internal static class ILHelpers
	{
		private readonly static HashSet<string> ilKeywords;

		static ILHelpers()
		{
			stackVariable1 = new String[199];
			stackVariable1[0] = "abstract";
			stackVariable1[1] = "algorithm";
			stackVariable1[2] = "alignment";
			stackVariable1[3] = "ansi";
			stackVariable1[4] = "any";
			stackVariable1[5] = "arglist";
			stackVariable1[6] = "array";
			stackVariable1[7] = "as";
			stackVariable1[8] = "assembly";
			stackVariable1[9] = "assert";
			stackVariable1[10] = "at";
			stackVariable1[11] = "auto";
			stackVariable1[12] = "autochar";
			stackVariable1[13] = "beforefieldinit";
			stackVariable1[14] = "blob";
			stackVariable1[15] = "blob_object";
			stackVariable1[16] = "bool";
			stackVariable1[17] = "brnull";
			stackVariable1[18] = "brnull.s";
			stackVariable1[19] = "brzero";
			stackVariable1[20] = "brzero.s";
			stackVariable1[21] = "bstr";
			stackVariable1[22] = "bytearray";
			stackVariable1[23] = "byvalstr";
			stackVariable1[24] = "callmostderived";
			stackVariable1[25] = "carray";
			stackVariable1[26] = "catch";
			stackVariable1[27] = "cdecl";
			stackVariable1[28] = "cf";
			stackVariable1[29] = "char";
			stackVariable1[30] = "cil";
			stackVariable1[31] = "class";
			stackVariable1[32] = "clsid";
			stackVariable1[33] = "const";
			stackVariable1[34] = "currency";
			stackVariable1[35] = "custom";
			stackVariable1[36] = "date";
			stackVariable1[37] = "decimal";
			stackVariable1[38] = "default";
			stackVariable1[39] = "demand";
			stackVariable1[40] = "deny";
			stackVariable1[41] = "endmac";
			stackVariable1[42] = "enum";
			stackVariable1[43] = "error";
			stackVariable1[44] = "explicit";
			stackVariable1[45] = "extends";
			stackVariable1[46] = "extern";
			stackVariable1[47] = "false";
			stackVariable1[48] = "famandassem";
			stackVariable1[49] = "family";
			stackVariable1[50] = "famorassem";
			stackVariable1[51] = "fastcall";
			stackVariable1[52] = "fault";
			stackVariable1[53] = "field";
			stackVariable1[54] = "filetime";
			stackVariable1[55] = "filter";
			stackVariable1[56] = "final";
			stackVariable1[57] = "finally";
			stackVariable1[58] = "fixed";
			stackVariable1[59] = "float";
			stackVariable1[60] = "float32";
			stackVariable1[61] = "float64";
			stackVariable1[62] = "forwardref";
			stackVariable1[63] = "fromunmanaged";
			stackVariable1[64] = "handler";
			stackVariable1[65] = "hidebysig";
			stackVariable1[66] = "hresult";
			stackVariable1[67] = "idispatch";
			stackVariable1[68] = "il";
			stackVariable1[69] = "illegal";
			stackVariable1[70] = "implements";
			stackVariable1[71] = "implicitcom";
			stackVariable1[72] = "implicitres";
			stackVariable1[73] = "import";
			stackVariable1[74] = "in";
			stackVariable1[75] = "inheritcheck";
			stackVariable1[76] = "init";
			stackVariable1[77] = "initonly";
			stackVariable1[78] = "instance";
			stackVariable1[79] = "int";
			stackVariable1[80] = "int16";
			stackVariable1[81] = "int32";
			stackVariable1[82] = "int64";
			stackVariable1[83] = "int8";
			stackVariable1[84] = "interface";
			stackVariable1[85] = "internalcall";
			stackVariable1[86] = "iunknown";
			stackVariable1[87] = "lasterr";
			stackVariable1[88] = "lcid";
			stackVariable1[89] = "linkcheck";
			stackVariable1[90] = "literal";
			stackVariable1[91] = "localloc";
			stackVariable1[92] = "lpstr";
			stackVariable1[93] = "lpstruct";
			stackVariable1[94] = "lptstr";
			stackVariable1[95] = "lpvoid";
			stackVariable1[96] = "lpwstr";
			stackVariable1[97] = "managed";
			stackVariable1[98] = "marshal";
			stackVariable1[99] = "method";
			stackVariable1[100] = "modopt";
			stackVariable1[101] = "modreq";
			stackVariable1[102] = "native";
			stackVariable1[103] = "nested";
			stackVariable1[104] = "newslot";
			stackVariable1[105] = "noappdomain";
			stackVariable1[106] = "noinlining";
			stackVariable1[107] = "nomachine";
			stackVariable1[108] = "nomangle";
			stackVariable1[109] = "nometadata";
			stackVariable1[110] = "noncasdemand";
			stackVariable1[111] = "noncasinheritance";
			stackVariable1[112] = "noncaslinkdemand";
			stackVariable1[113] = "noprocess";
			stackVariable1[114] = "not";
			stackVariable1[115] = "not_in_gc_heap";
			stackVariable1[116] = "notremotable";
			stackVariable1[117] = "notserialized";
			stackVariable1[118] = "null";
			stackVariable1[119] = "nullref";
			stackVariable1[120] = "object";
			stackVariable1[121] = "objectref";
			stackVariable1[122] = "opt";
			stackVariable1[123] = "optil";
			stackVariable1[124] = "out";
			stackVariable1[125] = "permitonly";
			stackVariable1[126] = "pinned";
			stackVariable1[127] = "pinvokeimpl";
			stackVariable1[128] = "prefix1";
			stackVariable1[129] = "prefix2";
			stackVariable1[130] = "prefix3";
			stackVariable1[131] = "prefix4";
			stackVariable1[132] = "prefix5";
			stackVariable1[133] = "prefix6";
			stackVariable1[134] = "prefix7";
			stackVariable1[135] = "prefixref";
			stackVariable1[136] = "prejitdeny";
			stackVariable1[137] = "prejitgrant";
			stackVariable1[138] = "preservesig";
			stackVariable1[139] = "private";
			stackVariable1[140] = "privatescope";
			stackVariable1[141] = "protected";
			stackVariable1[142] = "public";
			stackVariable1[143] = "record";
			stackVariable1[144] = "refany";
			stackVariable1[145] = "reqmin";
			stackVariable1[146] = "reqopt";
			stackVariable1[147] = "reqrefuse";
			stackVariable1[148] = "reqsecobj";
			stackVariable1[149] = "request";
			stackVariable1[150] = "retval";
			stackVariable1[151] = "rtspecialname";
			stackVariable1[152] = "runtime";
			stackVariable1[153] = "safearray";
			stackVariable1[154] = "sealed";
			stackVariable1[155] = "sequential";
			stackVariable1[156] = "serializable";
			stackVariable1[157] = "special";
			stackVariable1[158] = "specialname";
			stackVariable1[159] = "static";
			stackVariable1[160] = "stdcall";
			stackVariable1[161] = "storage";
			stackVariable1[162] = "stored_object";
			stackVariable1[163] = "stream";
			stackVariable1[164] = "streamed_object";
			stackVariable1[165] = "string";
			stackVariable1[166] = "struct";
			stackVariable1[167] = "synchronized";
			stackVariable1[168] = "syschar";
			stackVariable1[169] = "sysstring";
			stackVariable1[170] = "tbstr";
			stackVariable1[171] = "thiscall";
			stackVariable1[172] = "tls";
			stackVariable1[173] = "to";
			stackVariable1[174] = "true";
			stackVariable1[175] = "typedref";
			stackVariable1[176] = "unicode";
			stackVariable1[177] = "unmanaged";
			stackVariable1[178] = "unmanagedexp";
			stackVariable1[179] = "unsigned";
			stackVariable1[180] = "unused";
			stackVariable1[181] = "userdefined";
			stackVariable1[182] = "value";
			stackVariable1[183] = "valuetype";
			stackVariable1[184] = "vararg";
			stackVariable1[185] = "variant";
			stackVariable1[186] = "vector";
			stackVariable1[187] = "virtual";
			stackVariable1[188] = "void";
			stackVariable1[189] = "wchar";
			stackVariable1[190] = "winapi";
			stackVariable1[191] = "with";
			stackVariable1[192] = "wrapper";
			stackVariable1[193] = "property";
			stackVariable1[194] = "type";
			stackVariable1[195] = "flags";
			stackVariable1[196] = "callconv";
			stackVariable1[197] = "strict";
			stackVariable1[198] = "aggressiveinlining";
			ILHelpers.ilKeywords = ILHelpers.BuildKeywordList(stackVariable1);
			return;
		}

		internal static HashSet<string> BuildKeywordList(params string[] keywords)
		{
			V_0 = new HashSet<string>(keywords);
			V_1 = Type.GetTypeFromHandle(// 
			// Current member / type: System.Collections.Generic.HashSet`1<System.String> Telerik.JustDecompiler.Languages.IL.ILHelpers::BuildKeywordList(System.String[])
			// Exception in: System.Collections.Generic.HashSet<System.String> BuildKeywordList(System.String[])
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		internal static MemberMapping CreateCodeMapping(MethodDefinition member, CodeMappings codeMappings)
		{
			V_0 = new ILHelpers.u003cu003ec__DisplayClass5_0();
			V_0.member = member;
			if (V_0.member == null || !V_0.member.get_HasBody())
			{
				return null;
			}
			if (codeMappings == null)
			{
				return null;
			}
			V_1 = null;
			if (String.op_Equality(codeMappings.get_FullName(), V_0.member.get_DeclaringType().get_FullName()))
			{
				V_2 = codeMappings.get_Mapping();
				if (V_2.Find(new Predicate<MemberMapping>(V_0.u003cCreateCodeMappingu003eb__0)) == null)
				{
					stackVariable26 = new MemberMapping();
					V_3 = V_0.member.get_MetadataToken();
					stackVariable26.set_MetadataToken(V_3.ToInt32());
					stackVariable26.set_MemberReference(V_0.member.get_DeclaringType().Resolve());
					stackVariable26.set_MemberCodeMappings(new List<SourceCodeMapping>());
					stackVariable26.set_CodeSize(V_0.member.get_Body().get_CodeSize());
					V_1 = stackVariable26;
					V_2.Add(V_1);
				}
			}
			return V_1;
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
			if (!Char.IsLetter(identifier.get_Chars(0)) && !ILHelpers.IsValidIdentifierCharacter(identifier.get_Chars(0)))
			{
				if (String.op_Equality(identifier, ".ctor"))
				{
					return true;
				}
				return String.op_Equality(identifier, ".cctor");
			}
			V_0 = 1;
			while (V_0 < identifier.get_Length())
			{
				if (!Char.IsLetterOrDigit(identifier.get_Chars(V_0)) && !ILHelpers.IsValidIdentifierCharacter(identifier.get_Chars(V_0)) && identifier.get_Chars(V_0) != '.')
				{
					return false;
				}
				V_0 = V_0 + 1;
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
				if (String.op_Equality(typeName, "System.SByte"))
				{
					return "int8";
				}
				if (String.op_Equality(typeName, "System.Int16"))
				{
					return "int16";
				}
				if (String.op_Equality(typeName, "System.Int32"))
				{
					return "int32";
				}
				if (String.op_Equality(typeName, "System.Int64"))
				{
					return "int64";
				}
				if (String.op_Equality(typeName, "System.Byte"))
				{
					return "uint8";
				}
				if (String.op_Equality(typeName, "System.UInt16"))
				{
					return "uint16";
				}
				if (String.op_Equality(typeName, "System.UInt32"))
				{
					return "uint32";
				}
				if (String.op_Equality(typeName, "System.UInt64"))
				{
					return "uint64";
				}
				if (String.op_Equality(typeName, "System.Single"))
				{
					return "float32";
				}
				if (String.op_Equality(typeName, "System.Double"))
				{
					return "float64";
				}
				if (String.op_Equality(typeName, "System.Void"))
				{
					return "void";
				}
				if (String.op_Equality(typeName, "System.Boolean"))
				{
					return "bool";
				}
				if (String.op_Equality(typeName, "System.String"))
				{
					return "string";
				}
				if (String.op_Equality(typeName, "System.Char"))
				{
					return "char";
				}
				if (String.op_Equality(typeName, "System.Object"))
				{
					return "object";
				}
				if (String.op_Equality(typeName, "System.IntPtr"))
				{
					return "native int";
				}
			}
			return null;
		}

		internal static string ToInvariantCultureString(object value)
		{
			V_0 = value as IConvertible;
			if (V_0 == null)
			{
				return value.ToString();
			}
			return V_0.ToString(CultureInfo.get_InvariantCulture());
		}
	}
}