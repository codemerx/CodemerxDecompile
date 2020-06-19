using Mono.Cecil;
using System.Collections.Generic;
using Mono.Cecil.Extensions;
using System.Linq;

namespace Telerik.JustDecompiler.Languages.CSharp
{
    public class CSharpAttributeWriter : AttributeWriter
    {
        public CSharpAttributeWriter(NamespaceImperativeLanguageWriter writer)
            : base(writer)
        {
            attributesNotToShow.Add("System.Runtime.CompilerServices.DynamicAttribute");
            attributesNotToShow.Add("System.Runtime.CompilerServices.ExtensionAttribute");
        }

        protected override string OpeningBracket
        {
            get { return "["; }
        }

        protected override string ClosingBracket
        {
            get { return "]"; }
        }

        protected override string EqualsSign
        {
            get { return "="; }
        }

        protected override string ParameterAttributeSeparator
        {
            get { return string.Empty; }
        }

        public override void WriteMemberReturnValueAttributes(IMemberDefinition member)
        {
            List<ICustomAttribute> returnValueAttributes = this.GetSortedReturnValueAttributes(member);
            this.WriteAttributesInternal(member, returnValueAttributes, false, true);
        }

        protected override void WriteMemberAttributesInternal(IMemberDefinition member, bool isWinRTImplementation)
        {
            base.WriteMemberAttributesInternal(member, isWinRTImplementation);
            this.WriteMemberReturnValueAttributes(member);
        }

        protected override void WriteReturnValueAttributeKeyword()
        {
            this.genericWriter.WriteKeyword(this.genericWriter.KeyWordWriter.Return);
            this.genericWriter.Write(":");
            this.genericWriter.WriteSpace();
        }

        private List<ICustomAttribute> GetSortedReturnValueAttributes(IMemberDefinition member)
        {
            IMethodSignature method = null;
            TypeDefinition type = member as TypeDefinition;
            if (type != null && type.IsDelegate())
            {
                method = type.Methods.FirstOrDefault(m => m.Name == "Invoke");
            }
            else
            {
                method = member as IMethodSignature;
            }

            return base.GetSortedReturnValueAttributes(method);
        }
    }
}