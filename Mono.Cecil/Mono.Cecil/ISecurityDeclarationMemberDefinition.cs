//Telerik Authorship
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	public interface ISecurityDeclarationMemberDefinition : IMemberDefinition
	{
		Collection<SecurityDeclaration> SecurityDeclarations { get; }
	}
}
