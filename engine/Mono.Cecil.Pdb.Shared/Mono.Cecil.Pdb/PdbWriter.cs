//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;

using Mono.Cecil.Cil;
using Mono.Collections.Generic;

#if !READ_ONLY

namespace Mono.Cecil.Pdb {

	public class PdbWriter : Cil.ISymbolWriter {

		readonly ModuleDefinition module;
		readonly SymWriter writer;
		readonly Dictionary<string, SymDocumentWriter> documents;

		/*Telerik Authorship*/
		readonly Dictionary<MethodDefinition, Dictionary<VariableDefinition, string>> methodsVariableDefinitionToNameMap;

		internal PdbWriter (ModuleDefinition module, SymWriter writer,
			/*Telerik Authorship*/ Dictionary<MethodDefinition, Dictionary<VariableDefinition, string>> methodsVariableDefinitionToNameMap)
		{
			this.module = module;
			this.writer = writer;
			this.documents = new Dictionary<string, SymDocumentWriter> ();

			/*Telerik Authorship*/
			this.methodsVariableDefinitionToNameMap = methodsVariableDefinitionToNameMap;
		}

		public bool GetDebugHeader (out ImageDebugDirectory directory, out byte [] header)
		{
			header = writer.GetDebugInfo (out directory);
			return true;
		}

		public void Write(MethodBody body, /*Telerik Authorship*/ MetadataToken methodToken, /*Telerik Authorship*/ MetadataToken localVarToken)
		{
			/*Telerik Authorship*/
			//var method_token = body.Method.MetadataToken;
			var sym_token = new SymbolToken (methodToken.ToInt32 ());
			//var sym_token = new SymbolToken(body.Method.MetadataToken.ToInt32());

			var instructions = CollectInstructions (body);
			if (instructions.Count == 0)
				return;

			var start_offset = 0;
			var end_offset = body.CodeSize;

			writer.OpenMethod (sym_token);
			writer.OpenScope (start_offset);

			DefineSequencePoints (instructions);
			
			/*Telerik Authorship*/
			Dictionary<VariableDefinition, string> methodVariableDefinitionToNameMap =
				GetMethodVariableDefinitionToNameMap(body.Method);

			DefineVariables(body, /*Telerik Authorship*/ localVarToken, /*Telerik Authorship*/ methodVariableDefinitionToNameMap, start_offset, end_offset);

			writer.CloseScope (end_offset);
			writer.CloseMethod ();
		}

		/*Telerik Authorship*/
		private Dictionary<VariableDefinition, string> GetMethodVariableDefinitionToNameMap(MethodDefinition method)
		{
			Dictionary<VariableDefinition, string> result;

			if (!this.methodsVariableDefinitionToNameMap.TryGetValue(method, out result))
			{
				result = new Dictionary<VariableDefinition, string>();
			}

			return result;
		}

		Collection<Instruction> CollectInstructions (MethodBody body)
		{
			var collection = new Collection<Instruction> ();
			var instructions = body.Instructions;

			for (int i = 0; i < instructions.Count; i++) {
				var instruction = instructions [i];
				var sequence_point = instruction.SequencePoint;
				if (sequence_point == null)
					continue;

				GetDocument (sequence_point.Document);
				collection.Add (instruction);
			}

			return collection;
		}

		void DefineVariables(MethodBody body, /*Telerik Authorship*/ MetadataToken localVarToken,  /*Telerik Authorship*/ Dictionary<VariableDefinition, string> methodVariableDefinitionToNameMap, int start_offset, int end_offset)
		{
			if (!body.HasVariables)
				return;

			/*Telerik Authorship*/
			//var sym_token = new SymbolToken (body.LocalVarToken.ToInt32 ());
			var sym_token = new SymbolToken(localVarToken.ToInt32());

			var variables = body.Variables;
			for (int i = 0; i < variables.Count; i++) {
				var variable = variables [i];

				/*Telerik Authorship*/
				string variableName = GetVariableName(variable, methodVariableDefinitionToNameMap);

				CreateLocalVariable(variable, /*Telerik Authorship*/ variableName, sym_token, start_offset, end_offset);
			}
		}

		/*Telerik Authorship*/
		private string GetVariableName(VariableDefinition variable, Dictionary<VariableDefinition, string> methodVariableDefinitionToNameMap)
		{
			string variableName;

			if (!methodVariableDefinitionToNameMap.TryGetValue(variable, out variableName))
			{
				variableName = variable.Name;
			}

			return variableName;
		}

		void DefineSequencePoints (Collection<Instruction> instructions)
		{
			for (int i = 0; i < instructions.Count; i++) {
				var instruction = instructions [i];
				var sequence_point = instruction.SequencePoint;

				writer.DefineSequencePoints (
					GetDocument (sequence_point.Document),
					new [] { instruction.Offset },
					new [] { sequence_point.StartLine },
					new [] { sequence_point.StartColumn },
					new [] { sequence_point.EndLine },
					new [] { sequence_point.EndColumn });
			}
		}

		void CreateLocalVariable(VariableDefinition variable, /*Telerik Authorship*/ string variableName, SymbolToken local_var_token, int start_offset, int end_offset)
		{
			writer.DefineLocalVariable2 (
				/*Telerik Authorship*/ variableName,
				0,
				local_var_token,
				SymAddressKind.ILOffset,
				variable.Index,
				0,
				0,
				start_offset,
				end_offset);
		}

		SymDocumentWriter GetDocument (Document document)
		{
			if (document == null)
				return null;

			SymDocumentWriter doc_writer;
			if (documents.TryGetValue (document.Url, out doc_writer))
				return doc_writer;

			doc_writer = writer.DefineDocument (
				document.Url,
				document.Language.ToGuid (),
				document.LanguageVendor.ToGuid (),
				document.Type.ToGuid ());

			documents [document.Url] = doc_writer;
			return doc_writer;
		}

		public void Write (MethodSymbols symbols)
		{
			var sym_token = new SymbolToken (symbols.MethodToken.ToInt32 ());

			var start_offset = 0;
			var end_offset = symbols.CodeSize;

			writer.OpenMethod (sym_token);
			writer.OpenScope (start_offset);

			DefineSequencePoints (symbols);
			DefineVariables (symbols, start_offset, end_offset);

			writer.CloseScope (end_offset);
			writer.CloseMethod ();
		}

		void DefineSequencePoints (MethodSymbols symbols)
		{
			var instructions = symbols.instructions;

			for (int i = 0; i < instructions.Count; i++) {
				var instruction = instructions [i];
				var sequence_point = instruction.SequencePoint;

				writer.DefineSequencePoints (
					GetDocument (sequence_point.Document),
					new [] { instruction.Offset },
					new [] { sequence_point.StartLine },
					new [] { sequence_point.StartColumn },
					new [] { sequence_point.EndLine },
					new [] { sequence_point.EndColumn });
			}
		}

		void DefineVariables (MethodSymbols symbols, int start_offset, int end_offset)
		{
			if (!symbols.HasVariables)
				return;

			var sym_token = new SymbolToken (symbols.LocalVarToken.ToInt32 ());

			var variables = symbols.Variables;
			for (int i = 0; i < variables.Count; i++) {
				var variable = variables [i];

				CreateLocalVariable (variable, /*Telerik Authorship*/ variable.Name, sym_token, start_offset, end_offset);
			}
		}

		public void Dispose ()
		{
			var entry_point = module.EntryPoint;
			if (entry_point != null)
				writer.SetUserEntryPoint (new SymbolToken (entry_point.MetadataToken.ToInt32 ()));

			writer.Close ();
		}
	}
}

#endif
