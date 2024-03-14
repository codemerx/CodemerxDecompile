using Mono.Cecil.Cil;

namespace CodemerxDecompile.SearchResults;

public class InstructionSearchResult : SearchResult
{
    public required Instruction Instruction { get; init; }
}
