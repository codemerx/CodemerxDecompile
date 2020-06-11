using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Cecil.Extensions
{
	public static class SecurityDeclarationExtension
	{
		public static TypeReference GetSecurityActionTypeReference(this SecurityDeclaration self, ModuleDefinition module)
		{
			AssemblyNameReference mscorlib = module.ReferencedMscorlibRef();

			return new TypeReference("System.Security.Permissions", "SecurityAction", module, mscorlib);
		}
	}
}
