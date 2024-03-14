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
			this.Member = member;
			this.ControlFlowGraphs = new Dictionary<MethodDefinition, ControlFlowGraph>();
			this.MethodsVariableDefinitionToNameMap = new Dictionary<MethodDefinition, Dictionary<VariableDefinition, string>>();
			this.AutoImplementedProperties = new HashSet<PropertyDefinition>();
			this.MemberDefinitionToFoldingPositionMap = new Dictionary<IMemberDefinition, OffsetSpan>();
			this.ExceptionsWhileWriting = new HashSet<IMemberDefinition>();
			this.AutoImplementedEvents = new HashSet<EventDefinition>();
			this.MemberDeclarationToCodePostionMap = new Dictionary<IMemberDefinition, OffsetSpan>();
			this.MemberTokenToAttributesMap = new Dictionary<uint, OffsetSpan>();
			this.MemberTokenToDocumentationMap = new Dictionary<uint, OffsetSpan>();
			this.MemberTokenToDecompiledCodeMap = new Dictionary<uint, OffsetSpan>();
			this.MembersWithExceptions = new HashSet<uint>();
			this.GeneratedFilterMethods = new List<MethodDefinition>();
			this.CodeMappingInfo = new Telerik.JustDecompiler.Languages.CodeMappingInfo();
		}
	}
}