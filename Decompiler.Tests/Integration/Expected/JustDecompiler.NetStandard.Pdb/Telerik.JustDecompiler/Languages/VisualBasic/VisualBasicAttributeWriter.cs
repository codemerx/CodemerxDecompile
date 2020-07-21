using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Languages.VisualBasic
{
	public class VisualBasicAttributeWriter : AttributeWriter
	{
		protected override string ClosingBracket
		{
			get
			{
				return ">";
			}
		}

		protected override string EqualsSign
		{
			get
			{
				return ":=";
			}
		}

		protected override string OpeningBracket
		{
			get
			{
				return "<";
			}
		}

		protected override string ParameterAttributeSeparator
		{
			get
			{
				return " ";
			}
		}

		public VisualBasicAttributeWriter(NamespaceImperativeLanguageWriter writer)
		{
			base(writer);
			return;
		}

		private List<string> AddExtensionAttributeToIgnored(IEnumerable<string> attributesToIgnore)
		{
			V_0 = new List<string>();
			if (attributesToIgnore != null)
			{
				V_0.AddRange(attributesToIgnore);
			}
			V_0.Add("System.Runtime.CompilerServices.ExtensionAttribute");
			return V_0;
		}

		protected override CustomAttribute GetOutAttribute(ParameterDefinition parameter)
		{
			if (!parameter.IsOutParameter())
			{
				return this.GetOutAttribute(parameter);
			}
			return this.GetInOrOutAttribute(parameter, false);
		}

		public override void WriteAssemblyAttributes(AssemblyDefinition assembly, ICollection<string> attributesToIgnore = null)
		{
			this.WriteAssemblyAttributes(assembly, this.AddExtensionAttributeToIgnored(attributesToIgnore));
			return;
		}

		public override void WriteMemberAttributesAndNewLine(IMemberDefinition member, IEnumerable<string> ignored = null, bool isWinRTImplementation = false)
		{
			stackVariable1 = member;
			if (member as TypeDefinition != null)
			{
				stackVariable7 = this.AddExtensionAttributeToIgnored(ignored);
			}
			else
			{
				stackVariable7 = ignored;
			}
			this.WriteMemberAttributesAndNewLine(stackVariable1, stackVariable7, isWinRTImplementation);
			return;
		}

		public override void WriteMemberReturnValueAttributes(IMemberDefinition member)
		{
			V_0 = this.GetSortedReturnValueAttributes(member as IMethodSignature);
			this.WriteAttributesInternal(member, V_0, true, true);
			return;
		}

		protected override void WriteReturnValueAttributeKeyword()
		{
			return;
		}
	}
}