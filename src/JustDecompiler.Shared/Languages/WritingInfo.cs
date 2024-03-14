using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Languages
{
    public class WritingInfo
    {
        public WritingInfo(IMemberDefinition member)
        {
            this.Member = member;
			this.ControlFlowGraphs = new Dictionary<MethodDefinition, ControlFlowGraph>();
			this.MethodsVariableDefinitionToNameMap = new Dictionary<MethodDefinition, Dictionary<VariableDefinition, string>>();
			this.AutoImplementedProperties = new HashSet<PropertyDefinition>();
            MemberDefinitionToFoldingPositionMap = new Dictionary<IMemberDefinition, OffsetSpan>();
			this.ExceptionsWhileWriting = new HashSet<IMemberDefinition>();
			this.AutoImplementedEvents = new HashSet<EventDefinition>();
			this.MemberDeclarationToCodePostionMap = new Dictionary<IMemberDefinition, OffsetSpan>();
			/* AGPL */
			this.MemberDeclarationToCodeSpan = new Dictionary<IMemberDefinition, CodeSpan>();
			this.CodeMappingInfo = new CodeMappingInfo<CodeSpan>();
			/* End AGPL */
			this.MemberTokenToAttributesMap = new Dictionary<uint, OffsetSpan>();
			this.MemberTokenToDocumentationMap = new Dictionary<uint, OffsetSpan>();
			this.MemberTokenToDecompiledCodeMap = new Dictionary<uint, OffsetSpan>();
			this.MembersWithExceptions = new HashSet<uint>();
            this.GeneratedFilterMethods = new List<MethodDefinition>();
        }

        public IMemberDefinition Member { get; private set; }
		public Dictionary<MethodDefinition, ControlFlowGraph> ControlFlowGraphs { get; private set; }
		public Dictionary<MethodDefinition, Dictionary<VariableDefinition, string>> MethodsVariableDefinitionToNameMap { get; private set; }
        public HashSet<PropertyDefinition> AutoImplementedProperties { get; private set; }
		public HashSet<EventDefinition> AutoImplementedEvents { get; private set; }
		/// <summary>
		/// Main difference between this and <seealso cref="MemberDefinitionToCodePositionMap"/> is that in this dictionary
		/// the start offset is after the modifiers (i.e. after "public", "virtual", "readonly" and so on).
		/// </summary>
		public Dictionary<IMemberDefinition, OffsetSpan> MemberDeclarationToCodePostionMap { get; private set; }
		/* AGPL */
		public Dictionary<IMemberDefinition, CodeSpan> MemberDeclarationToCodeSpan { get; private set; }
		/* End AGPL */
		public Dictionary<IMemberDefinition, OffsetSpan> MemberDefinitionToFoldingPositionMap { get; set; }
		public Dictionary<uint, OffsetSpan> MemberTokenToDocumentationMap { get; private set; }
		public Dictionary<uint, OffsetSpan> MemberTokenToAttributesMap { get; private set; }
		public Dictionary<uint, OffsetSpan> MemberTokenToDecompiledCodeMap { get; private set; }
		public HashSet<IMemberDefinition> ExceptionsWhileWriting;
		public HashSet<uint> MembersWithExceptions { get; private set; }
        public IList<MethodDefinition> GeneratedFilterMethods { get; private set; }
		/* AGPL */
		public CodeMappingInfo<CodeSpan> CodeMappingInfo { get; private set; }
		/* End AGPL */
	}
}
