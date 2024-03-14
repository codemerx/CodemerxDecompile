namespace Mono.Cecil
{
	public interface IVisibilityDefinition
	{
		bool IsPrivate { get; set; }

		bool IsFamilyAndAssembly { get; set; }

		bool IsAssembly { get; set; }

		bool IsFamily { get; set; }

		bool IsFamilyOrAssembly { get; set; }

		bool IsPublic { get; set; }
	}
}