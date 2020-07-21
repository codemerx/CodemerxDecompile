using System;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal interface IBreaksContainer : ILogicalConstruct, ISingleEntrySubGraph, IComparable<ISingleEntrySubGraph>
	{

	}
}