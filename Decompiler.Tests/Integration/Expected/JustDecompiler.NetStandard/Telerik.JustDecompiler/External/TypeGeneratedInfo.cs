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
				return get_HasStructuralProblems();
			}
			set
			{
				set_HasStructuralProblems(value);
			}
		}

		// <HasStructuralProblems>k__BackingField
		private bool u003cHasStructuralProblemsu003ek__BackingField;

		public bool get_HasStructuralProblems()
		{
			return this.u003cHasStructuralProblemsu003ek__BackingField;
		}

		public void set_HasStructuralProblems(bool value)
		{
			this.u003cHasStructuralProblemsu003ek__BackingField = value;
			return;
		}

		public Dictionary<MemberIdentifier, CodeSpan> MemberMapping
		{
			get
			{
				return get_MemberMapping();
			}
			set
			{
				set_MemberMapping(value);
			}
		}

		// <MemberMapping>k__BackingField
		private Dictionary<MemberIdentifier, CodeSpan> u003cMemberMappingu003ek__BackingField;

		public Dictionary<MemberIdentifier, CodeSpan> get_MemberMapping()
		{
			return this.u003cMemberMappingu003ek__BackingField;
		}

		private void set_MemberMapping(Dictionary<MemberIdentifier, CodeSpan> value)
		{
			this.u003cMemberMappingu003ek__BackingField = value;
			return;
		}

		public IUniqueMemberIdentifier UniqueMemberIdentifier
		{
			get
			{
				return get_UniqueMemberIdentifier();
			}
			set
			{
				set_UniqueMemberIdentifier(value);
			}
		}

		// <UniqueMemberIdentifier>k__BackingField
		private IUniqueMemberIdentifier u003cUniqueMemberIdentifieru003ek__BackingField;

		public IUniqueMemberIdentifier get_UniqueMemberIdentifier()
		{
			return this.u003cUniqueMemberIdentifieru003ek__BackingField;
		}

		private void set_UniqueMemberIdentifier(IUniqueMemberIdentifier value)
		{
			this.u003cUniqueMemberIdentifieru003ek__BackingField = value;
			return;
		}

		public TypeGeneratedInfo(string fullPath, bool hasErrors, bool hasStructuralProblems, IUniqueMemberIdentifier uniqueMemberIdentifier, Dictionary<MemberIdentifier, CodeSpan> memberMapping)
		{
			base(fullPath, hasErrors);
			this.set_UniqueMemberIdentifier(uniqueMemberIdentifier);
			this.set_MemberMapping(memberMapping);
			this.set_HasStructuralProblems(hasStructuralProblems);
			return;
		}
	}
}