using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Languages
{
	public class WritingInfo
	{
		public HashSet<IMemberDefinition> ExceptionsWhileWriting;

		public HashSet<EventDefinition> AutoImplementedEvents
		{
			get;
			private set;
		}

		public HashSet<PropertyDefinition> AutoImplementedProperties
		{
			get;
			private set;
		}

		public Telerik.JustDecompiler.Languages.CodeMappingInfo CodeMappingInfo
		{
			get;
			private set;
		}

		public Dictionary<MethodDefinition, ControlFlowGraph> ControlFlowGraphs
		{
			get;
			private set;
		}

		public IList<MethodDefinition> GeneratedFilterMethods
		{
			get;
			private set;
		}

		public IMemberDefinition Member
		{
			get;
			private set;
		}

		public Dictionary<IMemberDefinition, OffsetSpan> MemberDeclarationToCodePostionMap
		{
			get;
			private set;
		}

		public Dictionary<IMemberDefinition, OffsetSpan> MemberDefinitionToFoldingPositionMap
		{
			get;
			set;
		}

		public HashSet<uint> MembersWithExceptions
		{
			get;
			private set;
		}

		public Dictionary<uint, OffsetSpan> MemberTokenToAttributesMap
		{
			get;
			private set;
		}

		public Dictionary<uint, OffsetSpan> MemberTokenToDecompiledCodeMap
		{
			get;
			private set;
		}

		public Dictionary<uint, OffsetSpan> MemberTokenToDocumentationMap
		{
			get;
			private set;
		}

		public Dictionary<MethodDefinition, Dictionary<VariableDefinition, string>> MethodsVariableDefinitionToNameMap
		{
			get;
			private set;
		}

		public WritingInfo(IMemberDefinition member)
		{
			base();
			this.set_Member(member);
			this.set_ControlFlowGraphs(new Dictionary<MethodDefinition, ControlFlowGraph>());
			this.set_MethodsVariableDefinitionToNameMap(new Dictionary<MethodDefinition, Dictionary<VariableDefinition, string>>());
			this.set_AutoImplementedProperties(new HashSet<PropertyDefinition>());
			this.set_MemberDefinitionToFoldingPositionMap(new Dictionary<IMemberDefinition, OffsetSpan>());
			this.ExceptionsWhileWriting = new HashSet<IMemberDefinition>();
			this.set_AutoImplementedEvents(new HashSet<EventDefinition>());
			this.set_MemberDeclarationToCodePostionMap(new Dictionary<IMemberDefinition, OffsetSpan>());
			this.set_MemberTokenToAttributesMap(new Dictionary<uint, OffsetSpan>());
			this.set_MemberTokenToDocumentationMap(new Dictionary<uint, OffsetSpan>());
			this.set_MemberTokenToDecompiledCodeMap(new Dictionary<uint, OffsetSpan>());
			this.set_MembersWithExceptions(new HashSet<uint>());
			this.set_GeneratedFilterMethods(new List<MethodDefinition>());
			this.set_CodeMappingInfo(new Telerik.JustDecompiler.Languages.CodeMappingInfo());
			return;
		}
	}
}