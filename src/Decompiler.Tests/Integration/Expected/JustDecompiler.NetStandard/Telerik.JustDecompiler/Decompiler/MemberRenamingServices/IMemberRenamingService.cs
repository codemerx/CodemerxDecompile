using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.MemberRenamingServices
{
	public interface IMemberRenamingService
	{
		MemberRenamingData GetMemberRenamingData(ModuleDefinition module);
	}
}