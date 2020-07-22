using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.MemberRenamingServices
{
	public class DefaultMemberRenamingService : IMemberRenamingService
	{

		private readonly ILanguage language;
		private readonly bool renameInvalidMembers;

		public DefaultMemberRenamingService(ILanguage language, bool renameInvalidMembers)
		{
			this.language = language;
			this.renameInvalidMembers = renameInvalidMembers;
		}

		public MemberRenamingData GetMemberRenamingData(ModuleDefinition module)
		{
			MemberRenamingData result = new MemberRenamingData();
			
			DoInitialRenamingAndEscaping(module, result);

			return result;
		}

		private void DoInitialRenamingAndEscaping(ModuleDefinition module, MemberRenamingData memberRenamingData)
		{
			foreach (TypeDefinition type in module.Types)
			{
				ProcessType(type, memberRenamingData);
			}
		}

		protected void ProcessType(TypeDefinition type, MemberRenamingData memberRenamingData)
		{
			RenameType(type, memberRenamingData);

			ProcessTypeMembers(type, memberRenamingData);

			if (type.HasNestedTypes)
			{
				foreach (TypeDefinition nestedType in type.NestedTypes)
				{
					ProcessType(nestedType, memberRenamingData);
				}
			}
		}

		private void ProcessTypeMembers(TypeDefinition type, MemberRenamingData memberRenamingData)
		{
			if (type.HasMethods)
			{
				foreach (MethodDefinition method in type.Methods)
				{
					RenameMember(method, memberRenamingData);
				}
			}

			if (type.HasProperties)
			{
				foreach (PropertyDefinition property in type.Properties)
				{
					RenameMember(property, memberRenamingData);
				}
			}

			if (type.HasFields)
			{
				foreach (FieldDefinition field in type.Fields)
				{
					RenameMember(field, memberRenamingData);
				}
			}

			if (type.HasEvents)
			{
				foreach (EventDefinition @event in type.Events)
				{
					RenameMember(@event, memberRenamingData);
				}
			}
		}

		protected void RenameType(TypeDefinition type, MemberRenamingData memberRenamingData)
		{
			string typeName = this.GetActualTypeName(type.Name);

			string typeNewName = typeName;
			if (this.renameInvalidMembers && !language.IsValidIdentifier(typeNewName))
			{
				typeNewName = language.ReplaceInvalidCharactersInIdentifier(typeNewName);
			}

			uint typeToken = type.MetadataToken.ToUInt32();

			if (typeName != typeNewName)
			{
				memberRenamingData.RenamedMembers.Add(typeToken);
			}

			if (language.IsGlobalKeyword(typeNewName))
			{
				typeNewName = language.EscapeWord(typeNewName);
			}

			memberRenamingData.RenamedMembersMap[typeToken] = typeNewName;
		}

        protected virtual string GetActualTypeName(string typeName)
        {
            return GenericHelper.GetNonGenericName(typeName);
        }

		private IMemberDefinition GetExplicitMemberDefinition(MemberReference member)
		{
			if (member is MethodDefinition)
			{
				return member as MethodDefinition;
			}

			if (member is PropertyDefinition)
			{
				return member as PropertyDefinition;
			}

			if (member is EventDefinition)
			{
				return member as EventDefinition;
			}

			// only methods, properties and events can be explicitly implemented
			return null;
		}

		private void RenameMember(MemberReference member, MemberRenamingData memberRenamingData)
		{
			uint memberToken = member.MetadataToken.ToUInt32();

			IMemberDefinition explicitMemberDefinition = GetExplicitMemberDefinition(member);
			if (explicitMemberDefinition != null && Utilities.IsExplicitInterfaceImplementataion(explicitMemberDefinition))
            {
                string explicitName = this.language.GetExplicitName(explicitMemberDefinition);
                memberRenamingData.RenamedMembersMap[memberToken] = Utilities.EscapeNameIfNeeded(explicitName, this.language);
            }
			else
			{
				string memberName = GenericHelper.GetNonGenericName(member.Name);

				string memberNewName = memberName;
				if (this.renameInvalidMembers && !language.IsValidIdentifier(memberNewName))
				{
					memberNewName = language.ReplaceInvalidCharactersInIdentifier(memberNewName);
				}

				if (memberName != memberNewName)
				{
					memberRenamingData.RenamedMembers.Add(memberToken);
				}

				memberNewName = Utilities.EscapeNameIfNeeded(memberNewName, language);

				memberRenamingData.RenamedMembersMap[memberToken] = memberNewName;
			}
		}
	}
}
