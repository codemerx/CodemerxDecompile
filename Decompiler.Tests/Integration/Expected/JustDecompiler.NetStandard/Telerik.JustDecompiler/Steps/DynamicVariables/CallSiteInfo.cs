using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Steps.DynamicVariables
{
	internal class CallSiteInfo
	{
		private List<int> dynamicArgumentIndices;

		public CallSiteBinderType BinderType
		{
			get;
			private set;
		}

		public FieldDefinition CallSiteField
		{
			get;
			private set;
		}

		public TypeReference ConvertType
		{
			get;
			set;
		}

		public List<int> DynamicArgumentIndices
		{
			get
			{
				stackVariable1 = this.dynamicArgumentIndices;
				if (stackVariable1 == null)
				{
					dummyVar0 = stackVariable1;
					stackVariable3 = new List<int>();
					V_0 = stackVariable3;
					this.dynamicArgumentIndices = stackVariable3;
					stackVariable1 = V_0;
				}
				return stackVariable1;
			}
		}

		public List<TypeReference> GenericTypeArguments
		{
			get;
			set;
		}

		public string MemberName
		{
			get;
			set;
		}

		public ExpressionType Operator
		{
			get;
			set;
		}

		public CallSiteInfo(FieldDefinition callSiteField, string binderMethodName)
		{
			base();
			this.set_CallSiteField(callSiteField);
			this.set_BinderType(this.GetBinderTypeFromName(binderMethodName));
			return;
		}

		private CallSiteBinderType GetBinderTypeFromName(string name)
		{
			if (name != null)
			{
				if (String.op_Equality(name, "BinaryOperation"))
				{
					return 0;
				}
				if (String.op_Equality(name, "Convert"))
				{
					return 1;
				}
				if (String.op_Equality(name, "GetIndex"))
				{
					return 2;
				}
				if (String.op_Equality(name, "GetMember"))
				{
					return 3;
				}
				if (String.op_Equality(name, "Invoke"))
				{
					return 4;
				}
				if (String.op_Equality(name, "InvokeConstructor"))
				{
					return 5;
				}
				if (String.op_Equality(name, "InvokeMember"))
				{
					return 6;
				}
				if (String.op_Equality(name, "IsEvent"))
				{
					return 7;
				}
				if (String.op_Equality(name, "SetIndex"))
				{
					return 8;
				}
				if (String.op_Equality(name, "SetMember"))
				{
					return 9;
				}
				if (String.op_Equality(name, "UnaryOperation"))
				{
					return 10;
				}
			}
			throw new Exception("Unknown CallSite binder.");
		}
	}
}