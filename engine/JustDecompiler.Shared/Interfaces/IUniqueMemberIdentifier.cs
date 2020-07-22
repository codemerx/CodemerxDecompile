using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JustDecompile.SmartAssembly.Attributes;

namespace Telerik.JustDecompiler.External.Interfaces
{
	/// <summary>
	/// Identifies uniquely a member by its module file path and its metadatatoken.
	/// </summary>
	[DoNotObfuscateType]
	[DoNotPruneType]
	public interface IUniqueMemberIdentifier
	{
		/// <summary>
		/// Filepath of the module containing the member.
		/// </summary>
		string ModuleFilePath { get; }

		/// <summary>
		/// The metadata token of the member.
		/// </summary>
		int MetadataToken { get; }
	}
}
