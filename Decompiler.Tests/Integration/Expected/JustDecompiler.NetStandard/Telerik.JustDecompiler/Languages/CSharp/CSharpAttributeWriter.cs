using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.CSharp
{
	public class CSharpAttributeWriter : AttributeWriter
	{
		protected override string ClosingBracket
		{
			get
			{
				return "]";
			}
		}

		protected override string EqualsSign
		{
			get
			{
				return "=";
			}
		}

		protected override string OpeningBracket
		{
			get
			{
				return "[";
			}
		}

		protected override string ParameterAttributeSeparator
		{
			get
			{
				return String.Empty;
			}
		}

		public CSharpAttributeWriter(NamespaceImperativeLanguageWriter writer)
		{
			base(writer);
			dummyVar0 = this.attributesNotToShow.Add("System.Runtime.CompilerServices.DynamicAttribute");
			dummyVar1 = this.attributesNotToShow.Add("System.Runtime.CompilerServices.ExtensionAttribute");
			return;
		}

		private List<ICustomAttribute> GetSortedReturnValueAttributes(IMemberDefinition member)
		{
			V_0 = null;
			V_1 = member as TypeDefinition;
			if (V_1 == null || !V_1.IsDelegate())
			{
				V_0 = member as IMethodSignature;
			}
			else
			{
				stackVariable12 = V_1.get_Methods();
				stackVariable13 = CSharpAttributeWriter.u003cu003ec.u003cu003e9__12_0;
				if (stackVariable13 == null)
				{
					dummyVar0 = stackVariable13;
					stackVariable13 = new Func<MethodDefinition, bool>(CSharpAttributeWriter.u003cu003ec.u003cu003e9.u003cGetSortedReturnValueAttributesu003eb__12_0);
					CSharpAttributeWriter.u003cu003ec.u003cu003e9__12_0 = stackVariable13;
				}
				V_0 = stackVariable12.FirstOrDefault<MethodDefinition>(stackVariable13);
			}
			return this.GetSortedReturnValueAttributes(V_0);
		}

		protected override void WriteMemberAttributesInternal(IMemberDefinition member, bool isWinRTImplementation)
		{
			this.WriteMemberAttributesInternal(member, isWinRTImplementation);
			this.WriteMemberReturnValueAttributes(member);
			return;
		}

		public override void WriteMemberReturnValueAttributes(IMemberDefinition member)
		{
			this.WriteAttributesInternal(member, this.GetSortedReturnValueAttributes(member), false, true);
			return;
		}

		protected override void WriteReturnValueAttributeKeyword()
		{
			this.genericWriter.WriteKeyword(this.genericWriter.get_KeyWordWriter().get_Return());
			this.genericWriter.Write(":");
			this.genericWriter.WriteSpace();
			return;
		}
	}
}