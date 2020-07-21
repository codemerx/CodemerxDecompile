using System;

namespace Telerik.JustDecompiler.Steps.DynamicVariables
{
	internal enum CallSiteBinderType
	{
		BinaryOperation,
		Convert,
		GetIndex,
		GetMember,
		Invoke,
		InvokeConstructor,
		InvokeMember,
		IsEvent,
		SetIndex,
		SetMember,
		UnaryOperation
	}
}