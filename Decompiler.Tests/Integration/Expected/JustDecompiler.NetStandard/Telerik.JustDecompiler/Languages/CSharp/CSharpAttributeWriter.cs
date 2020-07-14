using Mono.Cecil;
using Mono.Cecil.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public CSharpAttributeWriter(NamespaceImperativeLanguageWriter writer) : base(writer)
		{
			this.attributesNotToShow.Add("System.Runtime.CompilerServices.DynamicAttribute");
			this.attributesNotToShow.Add("System.Runtime.CompilerServices.ExtensionAttribute");
		}

		private List<ICustomAttribute> GetSortedReturnValueAttributes(IMemberDefinition member)
		{
			IMethodSignature methodSignature = null;
			TypeDefinition typeDefinition = member as TypeDefinition;
			if (typeDefinition == null || !typeDefinition.IsDelegate())
			{
				methodSignature = member as IMethodSignature;
			}
			else
			{
				methodSignature = typeDefinition.get_Methods().FirstOrDefault<MethodDefinition>((MethodDefinition m) => m.get_Name() == "Invoke");
			}
			return base.GetSortedReturnValueAttributes(methodSignature);
		}

		protected override void WriteMemberAttributesInternal(IMemberDefinition member, bool isWinRTImplementation)
		{
			base.WriteMemberAttributesInternal(member, isWinRTImplementation);
			this.WriteMemberReturnValueAttributes(member);
		}

		public override void WriteMemberReturnValueAttributes(IMemberDefinition member)
		{
			base.WriteAttributesInternal(member, this.GetSortedReturnValueAttributes(member), false, true);
		}

		protected override void WriteReturnValueAttributeKeyword()
		{
			this.genericWriter.WriteKeyword(this.genericWriter.KeyWordWriter.Return);
			this.genericWriter.Write(":");
			this.genericWriter.WriteSpace();
		}
	}
}