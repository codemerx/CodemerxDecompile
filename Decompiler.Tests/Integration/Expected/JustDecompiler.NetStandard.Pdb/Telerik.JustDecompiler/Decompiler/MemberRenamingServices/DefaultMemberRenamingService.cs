using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.MemberRenamingServices
{
	public class DefaultMemberRenamingService : IMemberRenamingService
	{
		private readonly ILanguage language;

		private readonly bool renameInvalidMembers;

		public DefaultMemberRenamingService(ILanguage language, bool renameInvalidMembers)
		{
			base();
			this.language = language;
			this.renameInvalidMembers = renameInvalidMembers;
			return;
		}

		private void DoInitialRenamingAndEscaping(ModuleDefinition module, MemberRenamingData memberRenamingData)
		{
			V_0 = module.get_Types().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.ProcessType(V_1, memberRenamingData);
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		protected virtual string GetActualTypeName(string typeName)
		{
			return GenericHelper.GetNonGenericName(typeName);
		}

		private IMemberDefinition GetExplicitMemberDefinition(MemberReference member)
		{
			if (member as MethodDefinition != null)
			{
				return member as MethodDefinition;
			}
			if (member as PropertyDefinition != null)
			{
				return member as PropertyDefinition;
			}
			if (member as EventDefinition == null)
			{
				return null;
			}
			return member as EventDefinition;
		}

		public MemberRenamingData GetMemberRenamingData(ModuleDefinition module)
		{
			V_0 = new MemberRenamingData();
			this.DoInitialRenamingAndEscaping(module, V_0);
			return V_0;
		}

		protected void ProcessType(TypeDefinition type, MemberRenamingData memberRenamingData)
		{
			this.RenameType(type, memberRenamingData);
			this.ProcessTypeMembers(type, memberRenamingData);
			if (type.get_HasNestedTypes())
			{
				V_0 = type.get_NestedTypes().GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						this.ProcessType(V_1, memberRenamingData);
					}
				}
				finally
				{
					V_0.Dispose();
				}
			}
			return;
		}

		private void ProcessTypeMembers(TypeDefinition type, MemberRenamingData memberRenamingData)
		{
			if (type.get_HasMethods())
			{
				V_0 = type.get_Methods().GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						this.RenameMember(V_1, memberRenamingData);
					}
				}
				finally
				{
					V_0.Dispose();
				}
			}
			if (type.get_HasProperties())
			{
				V_2 = type.get_Properties().GetEnumerator();
				try
				{
					while (V_2.MoveNext())
					{
						V_3 = V_2.get_Current();
						this.RenameMember(V_3, memberRenamingData);
					}
				}
				finally
				{
					V_2.Dispose();
				}
			}
			if (type.get_HasFields())
			{
				V_4 = type.get_Fields().GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						this.RenameMember(V_5, memberRenamingData);
					}
				}
				finally
				{
					V_4.Dispose();
				}
			}
			if (type.get_HasEvents())
			{
				V_6 = type.get_Events().GetEnumerator();
				try
				{
					while (V_6.MoveNext())
					{
						V_7 = V_6.get_Current();
						this.RenameMember(V_7, memberRenamingData);
					}
				}
				finally
				{
					V_6.Dispose();
				}
			}
			return;
		}

		private void RenameMember(MemberReference member, MemberRenamingData memberRenamingData)
		{
			V_0 = member.get_MetadataToken().ToUInt32();
			V_1 = this.GetExplicitMemberDefinition(member);
			if (V_1 != null && Utilities.IsExplicitInterfaceImplementataion(V_1))
			{
				V_3 = this.language.GetExplicitName(V_1);
				memberRenamingData.get_RenamedMembersMap().set_Item(V_0, Utilities.EscapeNameIfNeeded(V_3, this.language));
				return;
			}
			stackVariable10 = GenericHelper.GetNonGenericName(member.get_Name());
			V_4 = stackVariable10;
			if (this.renameInvalidMembers && !this.language.IsValidIdentifier(V_4))
			{
				V_4 = this.language.ReplaceInvalidCharactersInIdentifier(V_4);
			}
			if (String.op_Inequality(stackVariable10, V_4))
			{
				dummyVar0 = memberRenamingData.get_RenamedMembers().Add(V_0);
			}
			V_4 = Utilities.EscapeNameIfNeeded(V_4, this.language);
			memberRenamingData.get_RenamedMembersMap().set_Item(V_0, V_4);
			return;
		}

		protected void RenameType(TypeDefinition type, MemberRenamingData memberRenamingData)
		{
			stackVariable3 = this.GetActualTypeName(type.get_Name());
			V_0 = stackVariable3;
			if (this.renameInvalidMembers && !this.language.IsValidIdentifier(V_0))
			{
				V_0 = this.language.ReplaceInvalidCharactersInIdentifier(V_0);
			}
			V_1 = type.get_MetadataToken().ToUInt32();
			if (String.op_Inequality(stackVariable3, V_0))
			{
				dummyVar0 = memberRenamingData.get_RenamedMembers().Add(V_1);
			}
			if (this.language.IsGlobalKeyword(V_0))
			{
				V_0 = this.language.EscapeWord(V_0);
			}
			memberRenamingData.get_RenamedMembersMap().set_Item(V_1, V_0);
			return;
		}
	}
}