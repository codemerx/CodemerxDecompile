using Mono.Cecil;
using Mono.Cecil.Extensions;
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
		public TypeDeclarationsWriterContextService(bool renameInvalidMembers) : base(null, renameInvalidMembers)
		{
		}

		public override AssemblySpecificContext GetAssemblyContext(AssemblyDefinition assembly, ILanguage language)
		{
			return new AssemblySpecificContext();
		}

		private HashSet<EventDefinition> GetAutoImplementedEvents(TypeDefinition type, ILanguage language)
		{
			HashSet<EventDefinition> eventDefinitions = new HashSet<EventDefinition>();
			if (type.HasEvents)
			{
				foreach (EventDefinition @event in type.Events)
				{
					if (!(new AutoImplementedEventMatcher(@event, language)).IsAutoImplemented())
					{
						continue;
					}
					eventDefinitions.Add(@event);
				}
			}
			return eventDefinitions;
		}

		private Dictionary<string, Statement> GetDecompiledStatements(IMemberDefinition member, ILanguage language, IEnumerable<FieldDefinition> propertyFields)
		{
			Dictionary<string, Statement> strs = new Dictionary<string, Statement>();
			Queue<IMemberDefinition> memberDefinitions = new Queue<IMemberDefinition>();
			memberDefinitions.Enqueue(member);
			while (memberDefinitions.Count > 0)
			{
				IMemberDefinition memberDefinition = memberDefinitions.Dequeue();
				if (memberDefinition is TypeDefinition && memberDefinition == member)
				{
					foreach (IMemberDefinition typeMember in Utilities.GetTypeMembers(memberDefinition as TypeDefinition, language, true, null, null, null, propertyFields))
					{
						memberDefinitions.Enqueue(typeMember);
					}
				}
				if (memberDefinition is MethodDefinition)
				{
					strs.Add(Utilities.GetMemberUniqueName(memberDefinition), new BlockStatement());
				}
				if (memberDefinition is EventDefinition)
				{
					EventDefinition eventDefinition = memberDefinition as EventDefinition;
					if (eventDefinition.AddMethod != null)
					{
						strs.Add(Utilities.GetMemberUniqueName(eventDefinition.AddMethod), new BlockStatement());
					}
					if (eventDefinition.RemoveMethod != null)
					{
						strs.Add(Utilities.GetMemberUniqueName(eventDefinition.RemoveMethod), new BlockStatement());
					}
					if (eventDefinition.InvokeMethod != null)
					{
						strs.Add(Utilities.GetMemberUniqueName(eventDefinition.InvokeMethod), new BlockStatement());
					}
				}
				if (!(memberDefinition is PropertyDefinition))
				{
					continue;
				}
				PropertyDefinition propertyDefinition = memberDefinition as PropertyDefinition;
				if (propertyDefinition.GetMethod != null)
				{
					strs.Add(Utilities.GetMemberUniqueName(propertyDefinition.GetMethod), new BlockStatement());
				}
				if (propertyDefinition.SetMethod == null)
				{
					continue;
				}
				strs.Add(Utilities.GetMemberUniqueName(propertyDefinition.SetMethod), new BlockStatement());
			}
			return strs;
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
			TypeDefinition declaringTypeOrSelf = Utilities.GetDeclaringTypeOrSelf(member);
			Dictionary<FieldDefinition, PropertyDefinition> fieldToPropertyMap = declaringTypeOrSelf.GetFieldToPropertyMap(language);
			IEnumerable<FieldDefinition> keys = fieldToPropertyMap.Keys;
			HashSet<PropertyDefinition> propertyDefinitions = new HashSet<PropertyDefinition>(fieldToPropertyMap.Values);
			HashSet<EventDefinition> autoImplementedEvents = this.GetAutoImplementedEvents(declaringTypeOrSelf, language);
			TypeSpecificContext typeSpecificContext = new TypeSpecificContext(declaringTypeOrSelf)
			{
				AutoImplementedProperties = propertyDefinitions,
				AutoImplementedEvents = autoImplementedEvents
			};
			TypeDefinition typeDefinition = Utilities.GetDeclaringTypeOrSelf(member);
			Dictionary<string, string> strs = new Dictionary<string, string>();
			MemberRenamingData memberRenamingData = this.GetMemberRenamingData(typeDefinition.Module, language);
			ModuleSpecificContext moduleSpecificContext = new ModuleSpecificContext(typeDefinition.Module, new List<string>(), new Dictionary<string, List<string>>(), new Dictionary<string, HashSet<string>>(), strs, memberRenamingData.RenamedMembers, memberRenamingData.RenamedMembersMap);
			return new WriterContext(new AssemblySpecificContext(), moduleSpecificContext, typeSpecificContext, new Dictionary<string, MethodSpecificContext>(), this.GetDecompiledStatements(member, language, keys));
		}
	}
}