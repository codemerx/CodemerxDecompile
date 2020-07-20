using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.MemberRenamingServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.WriterContextServices
{
	public class TypeDeclarationsWriterContextService : BaseWriterContextService
	{
		public TypeDeclarationsWriterContextService(bool renameInvalidMembers)
		{
			base(null, renameInvalidMembers);
			return;
		}

		public override AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language)
		{
			return new AssemblySpecificContext();
		}

		private HashSet<EventDefinition> GetAutoImplementedEvents(TypeDefinition type, ILanguage language)
		{
			V_0 = new HashSet<EventDefinition>();
			if (type.get_HasEvents())
			{
				V_1 = type.get_Events().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (!(new AutoImplementedEventMatcher(V_2, language)).IsAutoImplemented())
						{
							continue;
						}
						dummyVar0 = V_0.Add(V_2);
					}
				}
				finally
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		private Dictionary<string, Statement> GetDecompiledStatements(IMemberDefinition member, ILanguage language, IEnumerable<FieldDefinition> propertyFields)
		{
			V_0 = new Dictionary<string, Statement>();
			V_1 = new Queue<IMemberDefinition>();
			V_1.Enqueue(member);
			while (V_1.get_Count() > 0)
			{
				V_2 = V_1.Dequeue();
				if (V_2 as TypeDefinition != null && (object)V_2 == (object)member)
				{
					V_3 = Utilities.GetTypeMembers(V_2 as TypeDefinition, language, true, null, null, null, propertyFields).GetEnumerator();
					try
					{
						while (V_3.MoveNext())
						{
							V_4 = V_3.get_Current();
							V_1.Enqueue(V_4);
						}
					}
					finally
					{
						((IDisposable)V_3).Dispose();
					}
				}
				if (V_2 as MethodDefinition != null)
				{
					V_0.Add(Utilities.GetMemberUniqueName(V_2), new BlockStatement());
				}
				if (V_2 as EventDefinition != null)
				{
					V_5 = V_2 as EventDefinition;
					if (V_5.get_AddMethod() != null)
					{
						V_0.Add(Utilities.GetMemberUniqueName(V_5.get_AddMethod()), new BlockStatement());
					}
					if (V_5.get_RemoveMethod() != null)
					{
						V_0.Add(Utilities.GetMemberUniqueName(V_5.get_RemoveMethod()), new BlockStatement());
					}
					if (V_5.get_InvokeMethod() != null)
					{
						V_0.Add(Utilities.GetMemberUniqueName(V_5.get_InvokeMethod()), new BlockStatement());
					}
				}
				if (V_2 as PropertyDefinition == null)
				{
					continue;
				}
				V_6 = V_2 as PropertyDefinition;
				if (V_6.get_GetMethod() != null)
				{
					V_0.Add(Utilities.GetMemberUniqueName(V_6.get_GetMethod()), new BlockStatement());
				}
				if (V_6.get_SetMethod() == null)
				{
					continue;
				}
				V_0.Add(Utilities.GetMemberUniqueName(V_6.get_SetMethod()), new BlockStatement());
			}
			return V_0;
		}

		public ModuleSpecificContext GetModuleContext(AssemblyDefinition assembly, ILanguage language)
		{
			return new ModuleSpecificContext();
		}

		public override ModuleSpecificContext GetModuleContext(ModuleDefinition module, ILanguage language)
		{
			return new ModuleSpecificContext();
		}

		public override WriterContext GetWriterContext(IMemberDefinition member, ILanguage language)
		{
			V_0 = Utilities.GetDeclaringTypeOrSelf(member);
			stackVariable4 = V_0.GetFieldToPropertyMap(language);
			V_1 = stackVariable4.get_Keys();
			V_2 = new HashSet<PropertyDefinition>(stackVariable4.get_Values());
			V_3 = this.GetAutoImplementedEvents(V_0, language);
			stackVariable13 = new TypeSpecificContext(V_0);
			stackVariable13.set_AutoImplementedProperties(V_2);
			stackVariable13.set_AutoImplementedEvents(V_3);
			V_4 = stackVariable13;
			V_5 = Utilities.GetDeclaringTypeOrSelf(member);
			V_6 = new Dictionary<string, string>();
			V_7 = this.GetMemberRenamingData(V_5.get_Module(), language);
			V_8 = new ModuleSpecificContext(V_5.get_Module(), new List<string>(), new Dictionary<string, List<string>>(), new Dictionary<string, HashSet<string>>(), V_6, V_7.get_RenamedMembers(), V_7.get_RenamedMembersMap());
			return new WriterContext(new AssemblySpecificContext(), V_8, V_4, new Dictionary<string, MethodSpecificContext>(), this.GetDecompiledStatements(member, language, V_1));
		}
	}
}