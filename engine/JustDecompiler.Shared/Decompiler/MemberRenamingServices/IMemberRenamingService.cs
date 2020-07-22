using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.MemberRenamingServices
{
	public interface IMemberRenamingService
	{
		MemberRenamingData GetMemberRenamingData(ModuleDefinition module);
	}
}
