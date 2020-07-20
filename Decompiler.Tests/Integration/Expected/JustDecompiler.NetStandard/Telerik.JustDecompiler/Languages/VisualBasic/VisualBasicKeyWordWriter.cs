using System;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.VisualBasic
{
	public class VisualBasicKeyWordWriter : IKeyWordWriter
	{
		public string AbstractMember
		{
			get
			{
				return "MustOverride";
			}
		}

		public string AbstractType
		{
			get
			{
				return "MustInherit";
			}
		}

		public string AddOn
		{
			get
			{
				return "AddHandler";
			}
		}

		public string AddressOf
		{
			get
			{
				return "AddressOf";
			}
		}

		public string As
		{
			get
			{
				return "As";
			}
		}

		public string Assembly
		{
			get
			{
				return "Assembly";
			}
		}

		public string Async
		{
			get
			{
				return "Async";
			}
		}

		public string Await
		{
			get
			{
				return "Await";
			}
		}

		public string Base
		{
			get
			{
				return "MyBase";
			}
		}

		public string ByRef
		{
			get
			{
				return "ByRef";
			}
		}

		public string ByVal
		{
			get
			{
				return "ByVal";
			}
		}

		public string Case
		{
			get
			{
				return "Case";
			}
		}

		public string Catch
		{
			get
			{
				return "Catch";
			}
		}

		public string Checked
		{
			get
			{
				return String.Empty;
			}
		}

		public string Class
		{
			get
			{
				return "Class";
			}
		}

		public string CollectionInitializer
		{
			get
			{
				return "From";
			}
		}

		public string Const
		{
			get
			{
				return "Const";
			}
		}

		public string Constructor
		{
			get
			{
				return "New";
			}
		}

		public string Contravariant
		{
			get
			{
				return "In";
			}
		}

		public string Covariant
		{
			get
			{
				return "Out";
			}
		}

		public string Custom
		{
			get
			{
				return "Custom";
			}
		}

		public string Default
		{
			get
			{
				return "Case";
			}
		}

		public string Delegate
		{
			get
			{
				return "Delegate";
			}
		}

		public string Dereference
		{
			get
			{
				return "*";
			}
		}

		public string Dim
		{
			get
			{
				return "Dim";
			}
		}

		public string Do
		{
			get
			{
				return "Do";
			}
		}

		public string Dynamic
		{
			get
			{
				return null;
			}
		}

		public string Else
		{
			get
			{
				return "Else";
			}
		}

		public string ElseIf
		{
			get
			{
				return "ElseIf";
			}
		}

		public string Enum
		{
			get
			{
				return "Enum";
			}
		}

		public string Event
		{
			get
			{
				return "Event";
			}
		}

		public string Explicit
		{
			get
			{
				return "Narrowing";
			}
		}

		public string ExtensionThis
		{
			get
			{
				return null;
			}
		}

		public string Extern
		{
			get
			{
				return null;
			}
		}

		public string False
		{
			get
			{
				return "False";
			}
		}

		public string Finally
		{
			get
			{
				return "Finally";
			}
		}

		public string Fire
		{
			get
			{
				return "RaiseEvent";
			}
		}

		public string Fixed
		{
			get
			{
				return "Fixed";
			}
		}

		public string ForEach
		{
			get
			{
				return "For Each";
			}
		}

		public string Function
		{
			get
			{
				return "Function";
			}
		}

		public string Get
		{
			get
			{
				return "Get";
			}
		}

		public string GoTo
		{
			get
			{
				return "GoTo";
			}
		}

		public string Hiding
		{
			get
			{
				return "Shadows";
			}
		}

		public string If
		{
			get
			{
				return "If";
			}
		}

		public string Implements
		{
			get
			{
				return "Implements";
			}
		}

		public string Implicit
		{
			get
			{
				return "Widening";
			}
		}

		public string In
		{
			get
			{
				return "In";
			}
		}

		public string Inherits
		{
			get
			{
				return "Inherits";
			}
		}

		public string Interface
		{
			get
			{
				return "Interface";
			}
		}

		public string Internal
		{
			get
			{
				return "Friend";
			}
		}

		public string Is
		{
			get
			{
				return "Is";
			}
		}

		public string IsType
		{
			get
			{
				return "TypeOf";
			}
		}

		public string Key
		{
			get
			{
				return "Key";
			}
		}

		public string LinqBy
		{
			get
			{
				return "By";
			}
		}

		public string LinqDescending
		{
			get
			{
				return "Descending";
			}
		}

		public string LinqEquals
		{
			get
			{
				return "Equals";
			}
		}

		public string LinqFrom
		{
			get
			{
				return "From";
			}
		}

		public string LinqGroup
		{
			get
			{
				return "Group";
			}
		}

		public string LinqIn
		{
			get
			{
				return "In";
			}
		}

		public string LinqInto
		{
			get
			{
				return "Into";
			}
		}

		public string LinqJoin
		{
			get
			{
				return "Join";
			}
		}

		public string LinqLet
		{
			get
			{
				return "Let";
			}
		}

		public string LinqOn
		{
			get
			{
				return "On";
			}
		}

		public string LinqOrderBy
		{
			get
			{
				return "Order By";
			}
		}

		public string LinqSelect
		{
			get
			{
				return "Select";
			}
		}

		public string LinqWhere
		{
			get
			{
				return "Where";
			}
		}

		public string Lock
		{
			get
			{
				return "SyncLock";
			}
		}

		public string LoopWhile
		{
			get
			{
				return "Loop While";
			}
		}

		public string Module
		{
			get
			{
				return "Module";
			}
		}

		public string Namespace
		{
			get
			{
				return "Namespace";
			}
		}

		public string NamespaceUsing
		{
			get
			{
				return "Imports";
			}
		}

		public string New
		{
			get
			{
				return "New";
			}
		}

		public string Next
		{
			get
			{
				return "Next";
			}
		}

		public string Null
		{
			get
			{
				return "Nothing";
			}
		}

		public string ObjectInitializer
		{
			get
			{
				return "With";
			}
		}

		public string Operator
		{
			get
			{
				return "Operator";
			}
		}

		public string Out
		{
			get
			{
				return "<Out>";
			}
		}

		public string Override
		{
			get
			{
				return "Overrides";
			}
		}

		public string ParamArray
		{
			get
			{
				return "ParamArray";
			}
		}

		public string Partial
		{
			get
			{
				return "Partial";
			}
		}

		public string Private
		{
			get
			{
				return "Private";
			}
		}

		public string Property
		{
			get
			{
				return "Property";
			}
		}

		public string Protected
		{
			get
			{
				return "Protected";
			}
		}

		public string Public
		{
			get
			{
				return "Public";
			}
		}

		public string ReadOnly
		{
			get
			{
				return "ReadOnly";
			}
		}

		public string ReDim
		{
			get
			{
				return "ReDim";
			}
		}

		public string RemoveOn
		{
			get
			{
				return "RemoveHandler";
			}
		}

		public string Return
		{
			get
			{
				return "Return";
			}
		}

		public string SealedMethod
		{
			get
			{
				return "NotOverridable";
			}
		}

		public string SealedType
		{
			get
			{
				return "NotInheritable";
			}
		}

		public string Set
		{
			get
			{
				return "Set";
			}
		}

		public string SizeOf
		{
			get
			{
				return "Marshal.SizeOf";
			}
		}

		public string Stackalloc
		{
			get
			{
				return "stackalloc";
			}
		}

		public string Static
		{
			get
			{
				return "Shared";
			}
		}

		public string StaticClass
		{
			get
			{
				return "Module";
			}
		}

		public string Struct
		{
			get
			{
				return "Structure";
			}
		}

		public string Sub
		{
			get
			{
				return "Sub";
			}
		}

		public string Switch
		{
			get
			{
				return "Select";
			}
		}

		public string Then
		{
			get
			{
				return "Then";
			}
		}

		public string This
		{
			get
			{
				return "Me";
			}
		}

		public string Throw
		{
			get
			{
				return "Throw";
			}
		}

		public string True
		{
			get
			{
				return "True";
			}
		}

		public string Try
		{
			get
			{
				return "Try";
			}
		}

		public string TryCast
		{
			get
			{
				return "TryCast";
			}
		}

		public string TypeOf
		{
			get
			{
				return "GetType";
			}
		}

		public string Unsafe
		{
			get
			{
				return String.Empty;
			}
		}

		public string Using
		{
			get
			{
				return "Using";
			}
		}

		public string Virtual
		{
			get
			{
				return "Overridable";
			}
		}

		public string Volatile
		{
			get
			{
				return "ModReq(IsVolatile)";
			}
		}

		public string When
		{
			get
			{
				return "When";
			}
		}

		public string While
		{
			get
			{
				return "While";
			}
		}

		public string WriteOnly
		{
			get
			{
				return "WriteOnly";
			}
		}

		public VisualBasicKeyWordWriter()
		{
			base();
			return;
		}
	}
}