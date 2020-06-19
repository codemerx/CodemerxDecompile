using System;
using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Steps.DynamicVariables
{
    internal class CallSiteInfo
    {
        private List<int> dynamicArgumentIndices;

        public List<int> DynamicArgumentIndices
        {
            get
            {
                return dynamicArgumentIndices ?? (dynamicArgumentIndices = new List<int>());
            }
        }

        public List<TypeReference> GenericTypeArguments { get; set; }

        public CallSiteBinderType BinderType { get; private set; }
        public FieldDefinition CallSiteField { get; private set; }

        public ExpressionType Operator { get; set; }
        public string MemberName { get; set; }
        public TypeReference ConvertType { get; set; }

        public CallSiteInfo(FieldDefinition callSiteField, string binderMethodName)
        {
            this.CallSiteField = callSiteField;
            this.BinderType = GetBinderTypeFromName(binderMethodName);
        }

        private CallSiteBinderType GetBinderTypeFromName(string name)
        {
            //Switch instead of Enum.Parse - performence improvements. (Enum.Parse contains unneeded checks)
            switch (name)
            {
                case "BinaryOperation":
                    return CallSiteBinderType.BinaryOperation;
                case "Convert":
                    return CallSiteBinderType.Convert;
                case "GetIndex":
                    return CallSiteBinderType.GetIndex;
                case "GetMember":
                    return CallSiteBinderType.GetMember;
                case "Invoke":
                    return CallSiteBinderType.Invoke;
                case "InvokeConstructor":
                    return CallSiteBinderType.InvokeConstructor;
                case "InvokeMember":
                    return CallSiteBinderType.InvokeMember;
                case "IsEvent":
                    return CallSiteBinderType.IsEvent;
                case "SetIndex":
                    return CallSiteBinderType.SetIndex;
                case "SetMember":
                    return CallSiteBinderType.SetMember;
                case "UnaryOperation":
                    return CallSiteBinderType.UnaryOperation;
                default:
                    throw new Exception("Unknown CallSite binder.");
            }
        }
    }
}
