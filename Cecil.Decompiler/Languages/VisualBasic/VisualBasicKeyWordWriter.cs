using System;

namespace Telerik.JustDecompiler.Languages.VisualBasic
{
    public class VisualBasicKeyWordWriter : IKeyWordWriter
    {
		public string Checked
		{
			get
			{
				/// The checked context is default for VB
				return string.Empty;
			}
		}

		public string Module
		{
			get { return "Module"; }
		}

		public string Partial
		{
			get { return "Partial"; }
		}

		public string Assembly
		{
            get { return "Assembly"; }
        }

        public string ReadOnly
        {
			get { return "ReadOnly"; }
		}

		public string Const
		{
			get { return "Const"; }
		}

		public string Then
		{
			get { return "Then"; }
		}

		public string TryCast
		{
			get { return "TryCast"; }
		}

		public string SizeOf
		{
			get { return "Marshal.SizeOf"; }
		}

		public string IsType
		{
			get { return "TypeOf"; }
		}

		public string Is
		{
			get { return "Is"; }
		}

		public string TypeOf
		{
			get { return "GetType"; }
		}

		public string Lock
		{
			get { return "SyncLock"; }
		}

		public string New
		{
			get { return "New"; }
		}

		public string Try
		{
			get { return "Try"; }
		}

		public string Catch
		{
			get { return "Catch"; }
		}

		public string Finally
		{
			get { return "Finally"; }
		}

		public string Fixed
		{
			get { return "Fixed"; }
		}

		public string ForEach
		{
			get { return "For Each"; }
		}

		public string In
		{
			get { return "In"; }
		}

		public string Next
		{
			get { return "Next"; }
		}

		public string Do
		{
			get { return "Do"; }
		}

		public string LoopWhile
		{
			get { return "Loop While"; }
		}

		public string Using
		{
			get { return "Using"; }
		}

		public string Namespace
		{
			get { return "Namespace"; }
		}

		public string NamespaceUsing
		{
			get { return "Imports"; }
		}

		public string While
		{
			get { return "While"; }
		}

		public string Volatile
		{
			get { return "ModReq(IsVolatile)"; }
		}

		public string As
		{
			get { return "As"; }
		}

		public string AddressOf
		{
			get { return "AddressOf"; }
		}

        public string Dereference
        {
            get { return "*"; }
        }

		public string Throw
		{
			get { return "Throw"; }
		}

		public string Default
		{
			get { return "Case"; }
		}

		public string Switch
		{
			get { return "Select"; }
		}

		public string Case
		{
			get { return "Case"; }
		}

		public string Return
		{
			get { return "Return"; }
		}

		public string If
		{
			get { return "If"; }
		}

        public string ElseIf
        {
            get { return "ElseIf"; }
        }

		public string Else
		{
			get { return "Else"; }
		}

		public string GoTo
		{
			get { return "GoTo"; }
		}

		public string Dim
		{
			get { return "Dim"; }
		}

		public string ReDim
		{
			get { return "ReDim"; }
		}

		public string Base
		{
			get { return "MyBase"; }
		}

		public string True
		{
			get { return "True"; }
		}

		public string False
		{
			get { return "False"; }
		}

		public string Null
		{
			get { return "Nothing"; }
		}

		public string Extern
		{
			get { return null; }
		}

		public string ByVal
		{
			get { return "ByVal"; }
		}

		public string ByRef
		{
			get { return "ByRef"; }
		}

		// TODO: This should be rendered as an attribute and not as a keyword.
		public string Out
		{
			get { return "<Out>"; }
		}

		// TODO: Render an attribute when having an extension method.
		public string ExtensionThis
		{
			get { return null; }
		}

		public string This
		{
			get { return "Me"; }
		}

		public string SealedMethod
		{
			get { return "NotOverridable"; }
		}

		public string Virtual
		{
			get { return "Overridable"; }
		}

		public string AbstractType
		{
			get { return "MustInherit"; }
		}

		public string AbstractMember
		{
			get { return "MustOverride"; }
		}

		public string Override
		{
			get { return "Overrides"; }
		}

		public string Sub
		{
			get { return "Sub"; }
		}

		public string Function
		{
			get { return "Function"; }
		}

		public string Constructor
		{
			get { return "New"; }
		}

		public string Get
		{
			get { return "Get"; }
		}

		public string Set
		{
			get { return "Set"; }
		}

		public string Property
		{
			get { return "Property"; }
		}

		public string Custom
		{
			get { return "Custom"; }
		}

		public string Event
		{
			get { return "Event"; }
		}

		public string Static
		{
			get { return "Shared"; }
		}

		public string Enum
		{
			get { return "Enum"; }
		}

		public string Struct
		{
			get { return "Structure"; }
		}

		public string Class
		{
			get { return "Class"; }
        }

        public string StaticClass
        {
            get { return "Module"; }
        }

        public string Interface
		{
			get { return "Interface"; }
		}

		public string SealedType
		{
			get { return "NotInheritable"; }
		}

		public string Public
		{
			get { return "Public"; }
		}

		public string Private
		{
			get { return "Private"; }
		}

		public string Protected
		{
			get { return "Protected"; }
		}

		public string Internal
		{
			get { return "Friend"; }
		}

        public string Operator
        {
            get { return "Operator"; }
        }

        public string Implicit
        {
            get { return "Widening"; }
        }

        public string Explicit
        {
            get { return "Narrowing"; }
        }
        
        public string Delegate
        {
            get { return "Delegate"; }
        }

        public string Unsafe
        {
            get { return string.Empty; }
        }

        public string WriteOnly
        {
            get { return "WriteOnly"; }
        }

        public string Covariant
        {
            get { return "Out"; }
        }

        public string Contravariant
        {
            get { return "In"; }
        }
		
		public string Dynamic
        {
            get { return null; }
        }

        public string AddOn
        {
            get { return "AddHandler"; }
        }

        public string RemoveOn
        {
            get { return "RemoveHandler"; }
        }

        public string Fire
        {
            get { return "RaiseEvent"; }
        }

        public string Key
        {
            get { return "Key"; }
        }

        public string ObjectInitializer
        {
            get { return "With"; }
        }

		public string CollectionInitializer
		{
			get { return "From"; }
		}

        public string Await
        {
            get { return "Await"; }
        }

        public string Async
        {
            get { return "Async"; }
        }

        public string ParamArray
        {
            get { return "ParamArray"; }
        }

		public string Hiding
		{
			get { return "Shadows"; }
		}

		public string Stackalloc 
		{
			get { return "stackalloc"; }
		}

		public string Implements
		{
			get { return "Implements"; }
		}

		public string Inherits
		{
			get { return "Inherits";  }
		}

        public string LinqFrom
        {
            get { return "From"; }
        }

        public string LinqIn
        {
            get { return "In"; }
        }

        public string LinqSelect
        {
            get { return "Select"; }
        }

        public string LinqInto
        {
            get { return "Into"; }
        }

        public string LinqWhere
        {
            get { return "Where"; }
        }

        public string LinqJoin
        {
            get { return "Join"; }
        }

        public string LinqOn
        {
            get { return "On"; }
        }

        public string LinqEquals
        {
            get { return "Equals"; }
        }

        public string LinqGroup
        {
            get { return "Group"; }
        }

        public string LinqBy
        {
            get { return "By"; }
        }

        public string LinqLet
        {
            get { return "Let"; }
        }

        public string LinqOrderBy
        {
            get { return "Order By"; }
        }

        public string LinqDescending
        {
            get { return "Descending"; }
        }

        public string When
        {
            get { return "When"; }
        }
    }
}