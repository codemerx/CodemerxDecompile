using Mono.Cecil;
using Mono.Cecil.Extensions;
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

		public VisualBasicAttributeWriter(NamespaceImperativeLanguageWriter writer) : base(writer)
		{
		}

		private List<string> AddExtensionAttributeToIgnored(IEnumerable<string> attributesToIgnore)
		{
			List<string> strs = new List<string>();
			if (attributesToIgnore != null)
			{
				strs.AddRange(attributesToIgnore);
			}
			strs.Add("System.Runtime.CompilerServices.ExtensionAttribute");
			return strs;
		}

		protected override CustomAttribute GetOutAttribute(ParameterDefinition parameter)
		{
			if (!parameter.IsOutParameter())
			{
				return base.GetOutAttribute(parameter);
			}
			return base.GetInOrOutAttribute(parameter, false);
		}

		public override void WriteAssemblyAttributes(AssemblyDefinition assembly, ICollection<string> attributesToIgnore = null)
		{
			base.WriteAssemblyAttributes(assembly, this.AddExtensionAttributeToIgnored(attributesToIgnore));
		}

		public override void WriteMemberAttributesAndNewLine(IMemberDefinition member, IEnumerable<string> ignored = null, bool isWinRTImplementation = false)
		{
			IEnumerable<string> strs;
			IMemberDefinition memberDefinition = member;
			if (member is TypeDefinition)
			{
				strs = this.AddExtensionAttributeToIgnored(ignored);
			}
			else
			{
				strs = ignored;
			}
			base.WriteMemberAttributesAndNewLine(memberDefinition, strs, isWinRTImplementation);
		}

		public override void WriteMemberReturnValueAttributes(IMemberDefinition member)
		{
			List<ICustomAttribute> sortedReturnValueAttributes = this.GetSortedReturnValueAttributes(member as IMethodSignature);
			base.WriteAttributesInternal(member, sortedReturnValueAttributes, true, true);
		}

		protected override void WriteReturnValueAttributeKeyword()
		{
		}
	}
}