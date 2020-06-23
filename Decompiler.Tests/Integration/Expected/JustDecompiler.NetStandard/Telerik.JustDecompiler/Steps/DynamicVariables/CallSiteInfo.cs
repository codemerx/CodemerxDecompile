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
				List<int> nums = this.dynamicArgumentIndices;
				if (nums == null)
				{
					List<int> nums1 = new List<int>();
					List<int> nums2 = nums1;
					this.dynamicArgumentIndices = nums1;
					nums = nums2;
				}
				return nums;
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
			this.CallSiteField = callSiteField;
			this.BinderType = this.GetBinderTypeFromName(binderMethodName);
		}

		private CallSiteBinderType GetBinderTypeFromName(string name)
		{
			if (name != null)
			{
				if (name == "BinaryOperation")
				{
					return CallSiteBinderType.BinaryOperation;
				}
				if (name == "Convert")
				{
					return CallSiteBinderType.Convert;
				}
				if (name == "GetIndex")
				{
					return CallSiteBinderType.GetIndex;
				}
				if (name == "GetMember")
				{
					return CallSiteBinderType.GetMember;
				}
				if (name == "Invoke")
				{
					return CallSiteBinderType.Invoke;
				}
				if (name == "InvokeConstructor")
				{
					return CallSiteBinderType.InvokeConstructor;
				}
				if (name == "InvokeMember")
				{
					return CallSiteBinderType.InvokeMember;
				}
				if (name == "IsEvent")
				{
					return CallSiteBinderType.IsEvent;
				}
				if (name == "SetIndex")
				{
					return CallSiteBinderType.SetIndex;
				}
				if (name == "SetMember")
				{
					return CallSiteBinderType.SetMember;
				}
				if (name == "UnaryOperation")
				{
					return CallSiteBinderType.UnaryOperation;
				}
			}
			throw new Exception("Unknown CallSite binder.");
		}
	}
}