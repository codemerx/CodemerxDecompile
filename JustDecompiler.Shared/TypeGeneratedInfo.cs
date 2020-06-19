using System.Collections.Generic;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.External
{
	public class TypeGeneratedInfo : FileGeneratedInfo, ITypeGeneratedInfo
	{
		/// <summary>
		/// The unique identifier of the item (class, resource) contained in the file.
		/// It consists of module's filepath and the metadata token of the item.
		/// </summary>
		public IUniqueMemberIdentifier UniqueMemberIdentifier { get; private set; }

		/// <summary>
		/// Contains the member to code position mapping for classes. For resources this will be null.
		/// </summary>
		public Dictionary<MemberIdentifier, CodeSpan> MemberMapping { get; private set; }

		/// <summary>
		/// Indicates if there are structural problems in the code, for instance missing closing braces.
		/// </summary>
		public bool HasStructuralProblems { get; set; }

		/// <param name="fullPath">The full filepath of the created file.</param>
		/// <param name="itemMetadataToken">The metadata token of the class contained in the file.</param>
		/// <param name="hasErrors">Signals if there was error during the generation of the file.</param>
		/// <param name="memberMapping">Contains the member to code position mapping for classes. For resources this will be null.</param>
		public TypeGeneratedInfo(string fullPath, bool hasErrors, bool hasStructuralProblems, IUniqueMemberIdentifier uniqueMemberIdentifier, Dictionary<MemberIdentifier, CodeSpan> memberMapping) : base(fullPath, hasErrors)
		{
			this.UniqueMemberIdentifier = uniqueMemberIdentifier;
			this.MemberMapping = memberMapping;
			this.HasStructuralProblems = hasStructuralProblems; 
		}
	}
}