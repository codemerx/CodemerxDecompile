using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
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

		private void DoInitialRenamingAndEscaping(ModuleDefinition module, MemberRenamingData memberRenamingData)
		{
			foreach (TypeDefinition type in module.get_Types())
			{
				this.ProcessType(type, memberRenamingData);
			}
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
			if (!(member is EventDefinition))
			{
				return null;
			}
			return member as EventDefinition;
		}

		public MemberRenamingData GetMemberRenamingData(ModuleDefinition module)
		{
			MemberRenamingData memberRenamingDatum = new MemberRenamingData();
			this.DoInitialRenamingAndEscaping(module, memberRenamingDatum);
			return memberRenamingDatum;
		}

		protected void ProcessType(TypeDefinition type, MemberRenamingData memberRenamingData)
		{
			this.RenameType(type, memberRenamingData);
			this.ProcessTypeMembers(type, memberRenamingData);
			if (type.get_HasNestedTypes())
			{
				foreach (TypeDefinition nestedType in type.get_NestedTypes())
				{
					this.ProcessType(nestedType, memberRenamingData);
				}
			}
		}

		private void ProcessTypeMembers(TypeDefinition type, MemberRenamingData memberRenamingData)
		{
			if (type.get_HasMethods())
			{
				foreach (MethodDefinition method in type.get_Methods())
				{
					this.RenameMember(method, memberRenamingData);
				}
			}
			if (type.get_HasProperties())
			{
				foreach (PropertyDefinition property in type.get_Properties())
				{
					this.RenameMember(property, memberRenamingData);
				}
			}
			if (type.get_HasFields())
			{
				foreach (FieldDefinition field in type.get_Fields())
				{
					this.RenameMember(field, memberRenamingData);
				}
			}
			if (type.get_HasEvents())
			{
				foreach (EventDefinition @event in type.get_Events())
				{
					this.RenameMember(@event, memberRenamingData);
				}
			}
		}

		private void RenameMember(MemberReference member, MemberRenamingData memberRenamingData)
		{
			uint num = member.get_MetadataToken().ToUInt32();
			IMemberDefinition explicitMemberDefinition = this.GetExplicitMemberDefinition(member);
			if (explicitMemberDefinition != null && Utilities.IsExplicitInterfaceImplementataion(explicitMemberDefinition))
			{
				string explicitName = this.language.GetExplicitName(explicitMemberDefinition);
				memberRenamingData.RenamedMembersMap[num] = Utilities.EscapeNameIfNeeded(explicitName, this.language);
				return;
			}
			string nonGenericName = GenericHelper.GetNonGenericName(member.get_Name());
			string str = nonGenericName;
			if (this.renameInvalidMembers && !this.language.IsValidIdentifier(str))
			{
				str = this.language.ReplaceInvalidCharactersInIdentifier(str);
			}
			if (nonGenericName != str)
			{
				memberRenamingData.RenamedMembers.Add(num);
			}
			str = Utilities.EscapeNameIfNeeded(str, this.language);
			memberRenamingData.RenamedMembersMap[num] = str;
		}

		protected void RenameType(TypeDefinition type, MemberRenamingData memberRenamingData)
		{
			string actualTypeName = this.GetActualTypeName(type.get_Name());
			string str = actualTypeName;
			if (this.renameInvalidMembers && !this.language.IsValidIdentifier(str))
			{
				str = this.language.ReplaceInvalidCharactersInIdentifier(str);
			}
			uint num = type.get_MetadataToken().ToUInt32();
			if (actualTypeName != str)
			{
				memberRenamingData.RenamedMembers.Add(num);
			}
			if (this.language.IsGlobalKeyword(str))
			{
				str = this.language.EscapeWord(str);
			}
			memberRenamingData.RenamedMembersMap[num] = str;
		}
	}
}