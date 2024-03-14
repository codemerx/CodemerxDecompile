using Mono.Collections.Generic;

namespace Mono.Cecil
{
	public interface IGenericDefinition
	{
		bool HasGenericParameters { get; }

		Collection<GenericParameter> GenericParameters { get; }

		string Name { get; }
	}
}