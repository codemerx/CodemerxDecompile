using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Languages.VisualBasic
{
	public class VisualBasicAttributeWriter : AttributeWriter
	{
		public VisualBasicAttributeWriter(NamespaceImperativeLanguageWriter writer) : base(writer)
		{
		}

		protected override string OpeningBracket
		{
			get	{ return "<"; }
		}

		protected override string ClosingBracket
		{
			get	{ return ">"; }
		}

		protected override string EqualsSign
		{
			get	{ return ":="; }
		}

        protected override string ParameterAttributeSeparator
        {
            get { return " "; }
        }

		protected override Mono.Cecil.CustomAttribute GetOutAttribute(ParameterDefinition parameter)
		{
			if (parameter.IsOutParameter())
			{
				return GetInOrOutAttribute(parameter, false);
			}

			return base.GetOutAttribute(parameter);
		}

        public override void WriteMemberAttributesAndNewLine(IMemberDefinition member, IEnumerable<string> ignored = null, bool isWinRTImplementation = false)
        {
            base.WriteMemberAttributesAndNewLine(member,
                                                 member is TypeDefinition ? AddExtensionAttributeToIgnored(ignored) : ignored,
                                                 isWinRTImplementation);
        }

        public override void WriteAssemblyAttributes(AssemblyDefinition assembly, ICollection<string> attributesToIgnore = null)
        {
            base.WriteAssemblyAttributes(assembly, AddExtensionAttributeToIgnored(attributesToIgnore));
        }

        private List<string> AddExtensionAttributeToIgnored(IEnumerable<string> attributesToIgnore)
        {
            List<string> ignoredAttributes = new List<string>();
            if (attributesToIgnore != null)
            {
                ignoredAttributes.AddRange(attributesToIgnore);
            }

            ignoredAttributes.Add("System.Runtime.CompilerServices.ExtensionAttribute");

            return ignoredAttributes;
        }

        protected override void WriteReturnValueAttributeKeyword()
        {
            // There is no need of return value attribute keyword in VB.NET
        }

        public override void WriteMemberReturnValueAttributes(IMemberDefinition member)
        {
            List<ICustomAttribute> returnValueAttributes = this.GetSortedReturnValueAttributes(member as IMethodSignature);
            base.WriteAttributesInternal(member, returnValueAttributes, true, true);
        }
    }
}