using System;

namespace Telerik.JustDecompiler.Languages.CSharp
{
    public class CSharpKeyWordWriter : IKeyWordWriter
    {
		public string Checked
		{
			get
			{
				return "checked";
			}
		}

		public string Module
		{
			get { return "module"; }
		}

		public string Partial
		{
			get { return "partial"; }
		}

		public string Assembly
		{
            get { return "assembly"; }
        }

        public string ReadOnly
        {
			get { return "readonly"; }
		}

		public string Const
		{
			get { return "const"; }
		}

		public string Then
		{
			get { return null; }
		}

		public string TryCast
		{
			get { return "as"; }
		}

		public string SizeOf
		{
			get { return "sizeof"; }
		}

		public string IsType
		{
			get { return null; }
		}

		public string Is
		{
			get { return "is"; }
		}

		public string TypeOf
		{
			get { return "typeof"; }
		}

		public string Lock
		{
			get { return "lock"; }
		}

		public string New
		{
			get { return "new"; }
		}

		public string Try
		{
			get { return "try"; }
		}

		public string Catch
		{
			get { return "catch"; }
		}

		public string Finally
		{
			get { return "finally"; }
		}

		public string Fixed
		{
			get { return "fixed"; }
		}

		public string ForEach
		{
			get { return "foreach"; }
		}

		public string In
		{
			get { return "in"; }
		}

		public string Next
		{
			get { return null; }
		}

		public string Do
		{
			get { return "do"; }
		}

		public string LoopWhile
		{
			get { return "while"; }
		}

		public string Using
		{
			get { return "using"; }
		}

		public string Namespace
		{
			get { return "namespace"; }
		}

		public string NamespaceUsing
		{
			get { return "using"; }
		}

		public string While
		{
			get { return "while"; }
		}

		public string Volatile
		{
			get { return "volatile"; }
		}

		public string As
		{
			get { return "as"; }
		}

		public string AddressOf
		{
			get { return "&"; }
		}

        public string Dereference
        {
            get { return "*"; }
        }

		public string Throw
		{
			get { return "throw"; }
		}

		public string Default
		{
			get { return "default"; }
		}

		public string Switch
		{
			get { return "switch"; }
		}

		public string Case
		{
			get { return "case"; }
		}

		public string Return
		{
			get { return "return"; }
		}

		public string If
		{
			get { return "if"; }
		}

        public string ElseIf
        {
            get { return "else if"; }
        }

		public string Else
		{
			get { return "else"; }
		}

		public string GoTo
		{
			get { return "goto"; }
		}

		public string Dim
		{
			get { return "var"; }
		}

		public string ReDim
		{
			get { return null; }
		}

		public string Base
		{
			get { return "base"; }
		}

		public string True
		{
			get { return "true"; }
		}

		public string False
		{
			get { return "false"; }
		}

		public string Null
		{
			get { return "null"; }
		}

		public string Extern
		{
			get { return "extern"; }
		}

		public string ByVal
		{
			get { return null; }
		}

		public string ByRef
		{
			get { return "ref"; }
		}

		public string Out
		{
			get { return "out"; }
		}

		public string ExtensionThis
		{
			get { return "this"; }
		}

		public string This
		{
			get { return "this"; }
		}

		public string SealedMethod
		{
			get { return "sealed"; }
		}

		public string Virtual
		{
			get { return "virtual"; }
		}

		public string AbstractType
		{
			get { return "abstract"; }
		}

		public string AbstractMember
		{
			get { return "abstract"; }
		}

		public string Override
		{
			get { return "override"; }
		}

		public string Sub
		{
			get { return null; }
		}

		public string Function
		{
			get { return null; }
		}

		public string Constructor
		{
			get { return null; }
		}

		public string Get
		{
			get { return "get"; }
		}

		public string Set
		{
			get { return "set"; }
		}

		public string Property
		{
			get { return null; }
		}

		public string Custom
		{
			get { return null; }
		}

		public string Event
		{
			get { return "event"; }
		}

		public string Static
		{
			get { return "static"; }
		}

		public string Enum
		{
			get { return "enum"; }
		}

		public string Struct
		{
			get { return "struct"; }
		}

		public string Class
		{
			get { return "class"; }
        }

        public string StaticClass
        {
            get { return "class"; }
        }

        public string Interface
		{
			get { return "interface"; }
		}

		public string SealedType
		{
			get { return "sealed"; }
		}

		public string Public
		{
			get { return "public"; }
		}

		public string Private
		{
			get { return "private"; }
		}

		public string Protected
		{
			get { return "protected"; }
		}

		public string Internal
		{
			get { return "internal"; }
		}

        public string Operator
        {
            get { return "operator"; }
        }

        public string Implicit
        {
            get { return "implicit"; }
        }

        public string Explicit
        {
            get { return "explicit"; }
        }

        public string Delegate
        {
            get { return "delegate"; }
        }

        public string Unsafe
        {
            get { return "unsafe"; }
        }

        public string WriteOnly
        {
            get { return null; }
        }

        public string Contravariant
        {
            get { return "in"; }
        }

        public string Covariant
        {
            get { return "out"; }
        }
		
		public string Dynamic
        {
            get { return "dynamic"; }
        }

        public string AddOn
        {
            get { return "add"; }
        }

        public string RemoveOn
        {
            get { return "remove"; }
        }

        public string Fire
        {
            get { return null; }
        }

        public string Key
        {
            get { return null; }
        }

        public string ObjectInitializer
        {
            get { return null; }
        }

		public string CollectionInitializer
		{
			get { return null; }
		}

        public string Await
        {
            get { return "await"; }
        }

        public string Async
        {
            get { return "async"; }
        }

        public string ParamArray
        {
            get { return "params"; }
        }

		public string Hiding
		{
			get { return "new"; }
		}

		public string Stackalloc 
		{
			get { return "stackalloc"; }
		}

		public string Implements
		{
			get { return null; }
		}

		public string Inherits
		{
			get { return null; }
		}
		
        public string LinqFrom
        {
            get { return "from"; }
        }

        public string LinqIn
        {
            get { return "in"; }
        }

        public string LinqSelect
        {
            get { return "select"; }
        }

        public string LinqInto
        {
            get { return "into"; }
        }

        public string LinqWhere
        {
            get { return "where"; }
        }

        public string LinqJoin
        {
            get { return "join"; }
        }

        public string LinqOn
        {
            get { return "on"; }
        }

        public string LinqEquals
        {
            get { return "equals"; }
        }

        public string LinqGroup
        {
            get { return "group"; }
        }

        public string LinqBy
        {
            get { return "by"; }
        }

        public string LinqLet
        {
            get { return "let"; }
        }

        public string LinqOrderBy
        {
            get { return "orderby"; }
        }

        public string LinqDescending
        {
            get { return "descending"; }
        }

        public string When
        {
            get { return "when"; }
        }
    }
}