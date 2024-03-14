using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.External
{
	public class TypeGeneratedInfo : FileGeneratedInfo, ITypeGeneratedInfo, IFileGeneratedInfo
	{
		public bool HasStructuralProblems
		{
			get
			{
				return JustDecompileGenerated_get_HasStructuralProblems();
			}
			set
			{
				JustDecompileGenerated_set_HasStructuralProblems(value);
			}
		}

		private bool JustDecompileGenerated_HasStructuralProblems_k__BackingField;

		public bool JustDecompileGenerated_get_HasStructuralProblems()
		{
			return this.JustDecompileGenerated_HasStructuralProblems_k__BackingField;
		}

		public void JustDecompileGenerated_set_HasStructuralProblems(bool value)
		{
			this.JustDecompileGenerated_HasStructuralProblems_k__BackingField = value;
		}

		public Dictionary<MemberIdentifier, CodeSpan> MemberMapping
		{
			get
			{
				return JustDecompileGenerated_get_MemberMapping();
			}
			set
			{
				JustDecompileGenerated_set_MemberMapping(value);
			}
		}

		private Dictionary<MemberIdentifier, CodeSpan> JustDecompileGenerated_MemberMapping_k__BackingField;

		public Dictionary<MemberIdentifier, CodeSpan> JustDecompileGenerated_get_MemberMapping()
		{
			return this.JustDecompileGenerated_MemberMapping_k__BackingField;
		}

		private void JustDecompileGenerated_set_MemberMapping(Dictionary<MemberIdentifier, CodeSpan> value)
		{
			this.JustDecompileGenerated_MemberMapping_k__BackingField = value;
		}

		public IUniqueMemberIdentifier UniqueMemberIdentifier
		{
			get
			{
				return JustDecompileGenerated_get_UniqueMemberIdentifier();
			}
			set
			{
				JustDecompileGenerated_set_UniqueMemberIdentifier(value);
			}
		}

		private IUniqueMemberIdentifier JustDecompileGenerated_UniqueMemberIdentifier_k__BackingField;

		public IUniqueMemberIdentifier JustDecompileGenerated_get_UniqueMemberIdentifier()
		{
			return this.JustDecompileGenerated_UniqueMemberIdentifier_k__BackingField;
		}

		private void JustDecompileGenerated_set_UniqueMemberIdentifier(IUniqueMemberIdentifier value)
		{
			this.JustDecompileGenerated_UniqueMemberIdentifier_k__BackingField = value;
		}

		public TypeGeneratedInfo(string fullPath, bool hasErrors, bool hasStructuralProblems, IUniqueMemberIdentifier uniqueMemberIdentifier, Dictionary<MemberIdentifier, CodeSpan> memberMapping) : base(fullPath, hasErrors)
		{
			this.UniqueMemberIdentifier = uniqueMemberIdentifier;
			this.MemberMapping = memberMapping;
			this.HasStructuralProblems = hasStructuralProblems;
		}
	}
}