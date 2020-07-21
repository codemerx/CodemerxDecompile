using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class SecurityDeclarationExtension
	{
		public static TypeReference GetSecurityActionTypeReference(this SecurityDeclaration self, ModuleDefinition module)
		{
			return new TypeReference("System.Security.Permissions", "SecurityAction", module, module.ReferencedMscorlibRef());
		}
	}
}