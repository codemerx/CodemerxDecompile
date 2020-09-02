using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.Decompiler;
using System.Text;

namespace Telerik.JustDecompiler.Languages.IL
{
    public class IntermediateLanguageWriter : BaseLanguageWriter
    {
        private CodeMappings CodeMappings { get; set; }
        private readonly FlagsWriter flagsWriter;
        private MethodDefinition method;

        private void WriteKeyWordPostSpace(string keyWord)
        {
            WriteKeyword(keyWord);
            WriteSpace();
        }

        private void WriteKeyWordPreSpace(string keyWord)
        {
            WriteSpace();
            WriteKeyword(keyWord);
        }

        private void WriteTokenPostSpace(string token)
        {
            WriteToken(token);
            WriteSpace();
        }

        private void WriteTokenPreSpace(string token)
        {
            WriteSpace();
            WriteToken(token);
        }

        private void WriteLiteralInQuotes(string literal)
        {
            WriteLiteral(string.Format("\"{0}\"", literal));
        }

        private void WriteEscaped(string identifier)
        {
            var escapedIdentifier = ILHelpers.Escape(identifier);
            if (escapedIdentifier.StartsWith("'"))
            {
                WriteKeyword(escapedIdentifier);
            }
            else
            {
                Write(escapedIdentifier);
            }
        }

        protected override void Write(EventDefinition @event)
        {
            WriteEventDeclaration(@event);

            int startIndex = formatter.CurrentPosition;

            WriteStartBlock();

            WriteAttributes(@event.CustomAttributes);

            WriteNestedMethod(".addon", @event.AddMethod);

            WriteLine();

            WriteNestedMethod(".removeon", @event.RemoveMethod);

            WriteNestedMethod(".fire", @event.InvokeMethod);

            foreach (var method in @event.OtherMethods)
            {
                WriteNestedMethod(".other", method);
            }
            WriteEndBlock();

            this.currentWritingInfo.MemberDefinitionToFoldingPositionMap[@event] = new OffsetSpan(startIndex, formatter.CurrentPosition - 1);
        }

        protected override void WriteEventDeclaration(EventDefinition @event)
        {
            WriteKeyWordPostSpace(".event");
            flagsWriter.WriteEventFlags(@event);
            WriteTypeReference(@event.EventType, ILNameSyntax.TypeName);
            WriteSpace();
            int startIndex = this.formatter.CurrentPosition;
            /* AGPL */
            CodeSpan codeSpan = this.Write(() => WriteReference(ILHelpers.Escape(@event.Name), @event));
            /* End AGPL */
            int endIndex = this.formatter.CurrentPosition - 1;
            /* AGPL */
            this.currentWritingInfo.MemberDeclarationToCodeSpan[@event] = codeSpan;
            /* End AGPL */
            this.currentWritingInfo.MemberDeclarationToCodePostionMap[@event] = new OffsetSpan(startIndex, endIndex);
        }

        protected override void Write(PropertyDefinition property)
        {
            WritePropertyDeclaration(property);

            int startIndex = formatter.CurrentPosition;

            WriteStartBlock();
            WriteAttributes(property.CustomAttributes);
            WriteNestedMethod(".get", property.GetMethod);
            if (property.GetMethod != null)
            {
                WriteLine();
            }
            WriteNestedMethod(".set", property.SetMethod);
            if (property.SetMethod != null)
            {
                WriteLine();
            }

            bool otherMethodWritten = false;
            foreach (MethodDefinition method in property.OtherMethods)
            {
                if (otherMethodWritten)
                {
                    WriteLine();
                }
                WriteNestedMethod(".other", method);

                otherMethodWritten = true;
            }
            if (otherMethodWritten)
            {
                WriteLine();
            }
            WriteEndBlock();

            currentWritingInfo.MemberDefinitionToFoldingPositionMap[property] = new OffsetSpan(startIndex, formatter.CurrentPosition - 1);
        }

        protected override void WritePropertyDeclaration(PropertyDefinition property)
        {
            WriteKeyWordPostSpace(".property");
            flagsWriter.WritePropertyFlags(property);
            if (property.HasThis)
            {
                WriteKeyWordPostSpace("instance");
            }
            WriteTypeReference(property.PropertyType);
            WriteSpace();

            int startIndex = this.formatter.CurrentPosition;
            /* AGPL */
            CodeSpan codeSpan = this.Write(() => WriteReference(ILHelpers.Escape(property.Name), property));
            /* End AGPL */
            int endIndex = this.formatter.CurrentPosition - 1;
            /* AGPL */
            this.currentWritingInfo.MemberDeclarationToCodeSpan[property] = codeSpan;
            /* End AGPL */
            this.currentWritingInfo.MemberDeclarationToCodePostionMap[property] = new OffsetSpan(startIndex, endIndex);

            WriteToken("(");
            if (property.HasParameters)
            {
                WriteLine();
                Indent();
                WriteParameters(property.Parameters);
                Outdent();
            }
            WriteToken(")");
        }

        protected override string WriteTypeDeclaration(TypeDefinition type, bool isPartial = false)
        {
            if (this.CodeMappings == null)
            {
                this.CodeMappings = new IL.CodeMappings { FullName = type.FullName, Mapping = new List<MemberMapping>() };
            }

            WriteKeyWordPostSpace(".class");

            if ((type.Attributes & TypeAttributes.ClassSemanticMask) == TypeAttributes.Interface)
            {
                WriteKeyWordPostSpace("interface");
            }

            flagsWriter.WriteTypeVisibility(type);
            flagsWriter.WriteTypeLayoutFlags(type);
            flagsWriter.WriteTypeStringFormat(type);
            flagsWriter.WriteTypeAttributes(type);

            int startIndex = this.formatter.CurrentPosition;
            /* AGPL */
            CodeSpan codeSpan = this.Write(() => WriteReference(ILHelpers.Escape(type.DeclaringType != null ? type.Name : type.FullName), type));
            /* End AGPL */
            int endIndex = this.formatter.CurrentPosition - 1;
            /* AGPL */
            this.currentWritingInfo.MemberDeclarationToCodeSpan[type] = codeSpan;
            /* End AGPL */
            this.currentWritingInfo.MemberDeclarationToCodePostionMap[type] = new OffsetSpan(startIndex, endIndex);
            WriteTypeParameters(type);
            WriteLine();

            if (type.BaseType != null)
            {
                Indent();
                WriteKeyWordPostSpace("extends");
                WriteTypeReference(type.BaseType, ILNameSyntax.TypeName);
                WriteLine();
                Outdent();
            }
            if (type.HasInterfaces)
            {
                Indent();
                for (int index = 0; index < type.Interfaces.Count; index++)
                {
                    if (index > 0)
                    {
                        WriteToken(",");
                        WriteLine();
                    }
                    if (index == 0)
                    {
                        WriteKeyWordPostSpace("implements");
                    }
                    else
                    {
                        Write("           ");
                    }
                    WriteTypeReference(type.Interfaces[index], ILNameSyntax.TypeName);
                }
                WriteLine();
                Outdent();
            }
            return string.Empty;
        }

        protected override void WriteTypeOpeningBlock(TypeDefinition type)
        {
            WriteAttributes(type.CustomAttributes);
            WriteSecurityDeclarations(type);
            if (type.HasLayoutInfo)
            {
                Write(string.Format(".pack {0}", type.PackingSize));
                WriteLine();
                Write(string.Format(".size {0}", type.ClassSize));
                WriteLine();
                WriteLine();
            }
        }

        protected override void WriteEndBlock(string statementName)
        {
            WriteToken("}");
        }

        protected override void WriteBeginBlock(bool inline = false)
        {
            if (inline)
            {
                WriteOpeningBrace();
                WriteSpace();
            }
            else
            {
                WriteOpeningBraceAndNewLine();
                Indent();
            }
        }

        private void WriteEndBlock()
        {
            Outdent();

            WriteToken("}");

            this.formatter.WriteEndBlock();

            //  WriteLine();
        }

        private void WriteStartBlock()
        {
            this.formatter.WriteStartBlock();

            WriteOpeningBraceBetweenLines();
            Indent();
        }

        private void WriteOpeningBraceBetweenLines()
        {
            WriteLine();
            WriteOpeningBraceAndNewLine();
        }

        private void WriteOpeningBraceAndNewLine()
        {
            //this.formatter.WriteStartBlock();

            WriteOpeningBrace();
            WriteLine();
        }

        private void WriteOpeningBrace()
        {
            WriteToken("{");
        }

        private void WriteNestedMethod(string keyword, MethodDefinition method)
        {
            if (method == null)
                return;

            WriteKeyWordPostSpace(keyword);

            WriteMethodReference(method, false);

            int startFoldingIndex = formatter.CurrentPosition;
            /* AGPL */
            CodeSpan codeSpan = this.Write(() =>
            {
                WriteStartBlock();

                WriteMethodBody(method);

                WriteEndBlock();
            });
            /* End AGPL */
            currentWritingInfo.MemberDefinitionToFoldingPositionMap[method] = new OffsetSpan(startFoldingIndex, formatter.CurrentPosition - 1);
            /* AGPL */
            currentWritingInfo.MemberDeclarationToCodeSpan[method] = codeSpan;
            /* End AGPL */
            currentWritingInfo.MemberDeclarationToCodePostionMap[method] = new OffsetSpan(startFoldingIndex, formatter.CurrentPosition - 1);
        }

        protected override void Write(FieldDefinition field)
        {

            if (TryWriteEnumField(field))
            {
                return;
            }

            WriteKeyWordPostSpace(".field");
            flagsWriter.WriteFieldVisibility(field);
            flagsWriter.WriteFieldFlags(field);
            if (field.HasMarshalInfo)
            {
                WriteMarshalInfo(field.MarshalInfo);
            }
            WriteTypeReference(field.FieldType);
            WriteSpace();
            int startOffset = this.formatter.CurrentPosition;
            CodeSpan codeSpan = this.Write(() => WriteReference(ILHelpers.Escape(field.Name), field));
            int endIndex = this.formatter.CurrentPosition - 1;
            this.currentWritingInfo.MemberDeclarationToCodeSpan[field] = codeSpan;
            this.currentWritingInfo.MemberDeclarationToCodePostionMap[field] = new OffsetSpan(startOffset, endIndex);
            if ((field.Attributes & FieldAttributes.HasFieldRVA) == FieldAttributes.HasFieldRVA)
            {
                Write(string.Format(" at I_{0:x8}", field.RVA));
            }
            if (field.HasConstant)
            {
                WriteSpace();
                WriteToken("=");
                WriteSpace();
                WriteConstant(field.Constant.Value);
            }
            if (field.HasCustomAttributes)
            {
                if (WriteAttributes(field.CustomAttributes))
                {
                    WriteLine();
                }
            }
        }

        protected override void Write(MethodDefinition method)
        {
            WriteMethodDeclaration(method);

            int startFoldingIndex = formatter.CurrentPosition;

            WriteStartBlock();

            //TODO: Consider collecting the analysis data in some way.
            this.method = method;

            WriteMethodBody(method);

            WriteEndBlock();

            currentWritingInfo.MemberDefinitionToFoldingPositionMap[method] = new OffsetSpan(startFoldingIndex, formatter.CurrentPosition - 1);
        }

        private void WriteMethodBody(MethodDefinition method)
        {
            WriteAttributes(method.CustomAttributes);
            if (method.HasOverrides)
            {
                foreach (var methodOverride in method.Overrides)
                {
                    WriteKeyWordPostSpace(".override");
                    WriteKeyWordPostSpace("method");
                    WriteMethodReference(methodOverride, true);
                    WriteLine();
                }
            }
            foreach (var parameter in method.Parameters)
            {
                WriteParameterAttributes(parameter);
            }
            WriteSecurityDeclarations(method);

            if (method.HasBody)
            {
                MemberMapping methodMapping = ILHelpers.CreateCodeMapping(method, this.CodeMappings);
                Disassemble(method.Body, methodMapping);
            }
        }

        protected override void WriteMethodDeclaration(MethodDefinition method, bool writeDocumentation = false)
        {
            WriteKeyWordPostSpace(".method");

            flagsWriter.WriteMethodVisibility(method);
            flagsWriter.WriteMethodFlags(method);

            if (method.IsCompilerControlled)
            {
                WriteKeyWordPostSpace("privatescope");
            }

            if ((method.Attributes & MethodAttributes.PInvokeImpl) == MethodAttributes.PInvokeImpl)
            {
                WriteKeyword("pinvokeimpl");
                if (method.HasPInvokeInfo && method.PInvokeInfo != null)
                {
                    WritePInvokeAttributes(method);
                }
                WriteSpace();
            }

            //WriteLine();
            Indent();
            if (method.ExplicitThis)
            {
                WriteKeyWordPostSpace("instance");
                WriteKeyWordPostSpace("explicit");
            }
            else if (method.HasThis)
            {
                WriteKeyWordPostSpace("instance");
            }

            flagsWriter.WriteMethodCallingConvention(method);

            WriteTypeReference(method.ReturnType);
            WriteSpace();
            if (method.MethodReturnType.HasMarshalInfo)
            {
                WriteMarshalInfo(method.MethodReturnType.MarshalInfo);
            }

            int startIndex = this.formatter.CurrentPosition;
            /* AGPL */
            CodeSpan codeSpan = this.Write(() => WriteMethodName(method));
            /* End AGPL */
            int endIndex = this.formatter.CurrentPosition - 1;
            /* AGPL */
            this.currentWritingInfo.MemberDeclarationToCodeSpan[method] = codeSpan;
            /* End AGPL */
            this.currentWritingInfo.MemberDeclarationToCodePostionMap[method] = new OffsetSpan(startIndex, endIndex);

            WriteTypeParameters(method);

            WriteMethodParameters(method);

            flagsWriter.WriteMethodCallType(method);
            WriteMethodManagedType(method);
            flagsWriter.WriteMethodImplementationAttribute(method);

            Outdent();
        }

        public override void WriteMemberNavigationName(object memberDefinition)
        {
            if (memberDefinition == null)
            {
                // happens when writing generic typereference member navigation path
                return;
            }
            if (memberDefinition is TypeReference)
            {
                TypeReference typeRef = (TypeReference)memberDefinition;

                this.formatter.Write(this.Settings.RenameInvalidMembers ? typeRef.GetFriendlyTypeName(Language) : typeRef.Name);
            }
            else
            {
                string value = GetFullName(memberDefinition);

                var startIndex = value.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

                if (startIndex.Length > 1)
                {
                    value = startIndex[1];
                }
                if (memberDefinition is MethodDefinition)
                {
                    MethodDefinition methodDef = memberDefinition as MethodDefinition;

                    this.formatter.Write(string.Format("{0} : {1}", value, this.Settings.RenameInvalidMembers ? methodDef.ReturnType.GetFriendlyFullName(Language) : methodDef.ReturnType.FullName));
                }
                else if (memberDefinition is PropertyDefinition)
                {
                    var property = memberDefinition as PropertyDefinition;

                    this.formatter.Write(string.Format("{0} : {1}", value, this.Settings.RenameInvalidMembers ? property.PropertyType.GetFriendlyFullName(Language) : property.PropertyType.FullName));
                }
                else if (memberDefinition is FieldDefinition)
                {
                    var field = memberDefinition as FieldDefinition;

                    this.formatter.Write(string.Format("{0} : {1}", value, this.Settings.RenameInvalidMembers ? field.FieldType.GetFriendlyFullName(Language) : field.FieldType.FullName));
                }
                else if (memberDefinition is EventDefinition)
                {
                    var @event = memberDefinition as EventDefinition;

                    this.formatter.Write(string.Format("{0} : {1}", value, this.Settings.RenameInvalidMembers ? @event.EventType.GetFriendlyFullName(Language) : @event.EventType.FullName));
                }
                else if (memberDefinition is ParameterReference)
                {
                    this.formatter.Write(((ParameterReference)memberDefinition).Name);
                }
                else if (memberDefinition is MemberReference)
                {
                    this.formatter.Write(this.Settings.RenameInvalidMembers ? ((MemberReference)memberDefinition).GetFriendlyFullName(Language) : ((MemberReference)memberDefinition).FullName);
                }
            }
        }

        public override void WriteMemberNavigationPathFullName(object member)
        {
            if (member == null)
            {
                // happens when writing generic typereference member navigation path
                return;
            }

            if (member is TypeReference)
            {
                this.formatter.Write(((TypeReference)member).Name);
            }
            else
            {
                string value = GetName(member);

                var startIndex = value.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

                if (startIndex.Length > 1)
                {
                    value = startIndex[1];
                }
                if (member is ParameterReference)
                {
                    this.formatter.Write(((ParameterReference)member).Name);
                }
                else if (member is MemberReference)
                {
                    this.formatter.Write(((MemberReference)member).GetFriendlyFullName(Language));
                }
            }
        }

        //public override void WriteMemberEscapedOnlyName(object memberDefinition)
        //{
        //    if (memberDefinition == null)
        //    {
        //        // happens when writing generic typereference member navigation path
        //        return;
        //    }
        //    string name = GetMemberEscapedOnlyName(memberDefinition, "<", ">");

        //    this.formatter.Write(name);

        //    //if (memberDefinition is TypeReference)
        //    //{
        //    //    TypeReference typeRef = memberDefinition as TypeReference;

        //    //    this.formatter.Write(typeRef.GetFriendlyTypeName(Language));
        //    //}
        //    //else
        //    //{
        //    //    if (memberDefinition is IMemberDefinition)
        //    //    {
        //    //        string name = ((IMemberDefinition)memberDefinition).GetFriendlyMemberName(Language);

        //    //        this.formatter.Write(name);
        //    //    }
        //    //    else
        //    //    {
        //    //        string value = GetName(memberDefinition);

        //    //        var startIndex = value.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

        //    //        if (startIndex.Length > 1)
        //    //        {
        //    //            value = startIndex[1];
        //    //        }
        //    //        StringBuilder stringBuilder = new StringBuilder();

        //    //        string[] nameParts = value.Split('.');

        //    //        for (int i = 0; i < nameParts.Length; i++)
        //    //        {
        //    //            string namePart = nameParts[i];

        //    //            stringBuilder.Append(GenericHelper.ReplaceInvalidCharactersName(Language, namePart));
        //    //            if (i < nameParts.Length - 1)
        //    //            {
        //    //                stringBuilder.Append(".");
        //    //            }
        //    //        }
        //    //        this.formatter.Write(stringBuilder.ToString());
        //    //    }
        //    //}
        //}

        private string GetFullName(object member)
        {
            string name = string.Empty;

            if (member is IMemberDefinition)
            {
                name = this.Settings.RenameInvalidMembers ? ((IMemberDefinition)member).GetFriendlyFullName(Language) : ((IMemberDefinition)member).FullName;
            }
            else if (member is ParameterReference)
            {
                name = this.Settings.RenameInvalidMembers ? Language.ReplaceInvalidCharactersInIdentifier(((ParameterReference)member).Name) : ((ParameterReference)member).Name;
            }
            else if (member is MemberReference)
            {
                name = this.Settings.RenameInvalidMembers ? ((MemberReference)member).GetFriendlyFullName(Language) : ((MemberReference)member).FullName;
            }
            else
            {
                name = this.Settings.RenameInvalidMembers ? Language.ReplaceInvalidCharactersInIdentifier(member.ToString()) : member.ToString();
            }
            return name;
        }

        private string GetName(object member)
        {
            string name = string.Empty;

            if (member is IMemberDefinition)
            {
                name = ((IMemberDefinition)member).Name;
            }
            else if (member is ParameterReference)
            {
                name = ((ParameterReference)member).Name;
            }
            else if (member is MemberReference)
            {
                name = ((MemberReference)member).Name;
            }
            else
            {
                name = member.ToString();
            }
            return name;
        }

        private void WriteMethodParameters(MethodDefinition method)
        {
            WriteTokenPreSpace("(");
            if (method.HasParameters)
            {
                WriteLine();
                Indent();
                WriteParameters(method.Parameters);
                Outdent();
            }
            WriteTokenPostSpace(")");
        }

        private void WriteMethodName(MethodDefinition method)
        {
            if (method.IsCompilerControlled)
            {
                WriteReference(ILHelpers.Escape(method.Name + "$PST" + method.MetadataToken.ToInt32().ToString("X8")), method);
            }
            else
            {
                WriteReference(ILHelpers.Escape(method.Name), method);
            }
        }

        private void WriteMethodManagedType(MethodDefinition method)
        {
            if ((method.ImplAttributes & MethodImplAttributes.ManagedMask) == MethodImplAttributes.Managed)
            {
                WriteKeyWordPostSpace("managed");
            }
            else
            {
                WriteKeyWordPostSpace("unmanaged");
            }
        }

        private void WritePInvokeAttributes(MethodDefinition method)
        {
            PInvokeInfo info = method.PInvokeInfo;
            WriteToken("(");
            WriteLiteralInQuotes(ConvertString(info.Module.Name));
            WriteToken(")");

            if (!string.IsNullOrEmpty(info.EntryPoint) && info.EntryPoint != method.Name)
            {
                WriteSpace();
                WriteKeyword("as");
                WriteSpace();
                WriteLiteralInQuotes(ConvertString(info.EntryPoint));
            }

            if (info.IsNoMangle)
                WriteKeyWordPreSpace("nomangle");

            if (info.IsCharSetAnsi)
                WriteKeyWordPreSpace("ansi");
            else if (info.IsCharSetAuto)
                WriteKeyWordPreSpace("autochar");
            else if (info.IsCharSetUnicode)
                WriteKeyWordPreSpace("unicode");

            if (info.SupportsLastError)
                WriteKeyWordPreSpace("lasterr");

            if (info.IsCallConvCdecl)
                WriteKeyWordPreSpace("cdecl");
            else if (info.IsCallConvFastcall)
                WriteKeyWordPreSpace("fastcall");
            else if (info.IsCallConvStdCall)
                WriteKeyWordPreSpace("stdcall");
            else if (info.IsCallConvThiscall)
                WriteKeyWordPreSpace("thiscall");
            else if (info.IsCallConvWinapi)
                WriteKeyWordPreSpace("winapi");

            WriteToken(")");
        }

        private void Disassemble(MethodBody body, MemberMapping methodMapping)
        {
            MethodDefinition method = body.Method;
            if (method.DeclaringType.Module.Assembly.EntryPoint == method)
            {
                WriteKeyword(".entrypoint");
                WriteLine();
            }

            if (method.Body.HasVariables)
            {
                WriteMethodVariables(method);
                WriteLine();
            }

            if (this.Settings.ShouldGenerateBlocks && body.Instructions.Count > 0)
            {
                Instruction inst = body.Instructions[0];
                HashSet<int> branchTargets = GetBranchTargets(body.Instructions);
                WriteStructureBody(new ILBlock(body), branchTargets, ref inst, methodMapping, method.Body.CodeSize);
            }
            else
            {
                foreach (var inst in method.Body.Instructions)
                {
                    WriteInstruction(inst);

                    if (methodMapping != null)
                    {
                        methodMapping.MemberCodeMappings.Add(
                            new SourceCodeMapping()
                            {
                                ILInstructionOffset = new ILRange { From = inst.Offset, To = inst.Next == null ? method.Body.CodeSize : inst.Next.Offset },
                                MemberMapping = methodMapping
                            });
                    }

                    WriteLine();
                }
                if (method.Body.HasExceptionHandlers)
                {
                    WriteLine();
                    foreach (var eh in method.Body.ExceptionHandlers)
                    {
                        WriteExceptionHandler(eh);
                        WriteLine();
                    }
                }
            }
        }

        private void WriteMethodVariables(MethodDefinition method)
        {
            WriteKeyWordPostSpace(".locals");
            if (method.Body.InitLocals)
            {
                WriteKeyWordPostSpace("init");
            }
            WriteToken("(");
            WriteLine();
            Indent();
            foreach (var v in method.Body.Variables)
            {
                WriteDefinition("[" + v.Index + "] ", v);
                WriteTypeReference(v.VariableType);
                if (!string.IsNullOrEmpty(v.Name))
                {
                    WriteSpace();
                    Write(ILHelpers.Escape(v.Name));
                }
                if (v.Index + 1 < method.Body.Variables.Count)
                {
                    WriteToken(",");
                }
                WriteLine();
            }
            Outdent();
            WriteToken(")");
            WriteLine();
        }

        private void WriteStructureBody(ILBlock s, HashSet<int> branchTargets, ref Instruction inst, MemberMapping currentMethodMapping, int codeSize)
        {
            bool isFirstInstructionInStructure = true;
            bool prevInstructionWasBranch = false;
            int childIndex = 0;
            while (inst != null && inst.Offset < s.EndOffset)
            {
                int offset = inst.Offset;
                if (childIndex < s.Children.Count && s.Children[childIndex].StartOffset <= offset && offset < s.Children[childIndex].EndOffset)
                {
                    ILBlock child = s.Children[childIndex++];
                    WriteStructureHeader(child);
                    WriteStructureBody(child, branchTargets, ref inst, currentMethodMapping, codeSize);
                    WriteStructureFooter(child);
                }
                else
                {
                    if (!isFirstInstructionInStructure && (prevInstructionWasBranch || branchTargets.Contains(offset)))
                    {
                        WriteLine();
                    }
                    WriteInstruction(inst);

                    if (currentMethodMapping != null)
                    {
                        currentMethodMapping.MemberCodeMappings.Add(
                            new SourceCodeMapping()
                            {
                                ILInstructionOffset = new ILRange { From = inst.Offset, To = inst.Next == null ? codeSize : inst.Next.Offset },
                                MemberMapping = currentMethodMapping
                            });
                    }

                    WriteLine();

                    prevInstructionWasBranch = (inst.OpCode.FlowControl == FlowControl.Branch) ||
                                               (inst.OpCode.FlowControl == FlowControl.Cond_Branch) ||
                                               (inst.OpCode.FlowControl == FlowControl.Return) ||
                                               (inst.OpCode.FlowControl == FlowControl.Throw);

                    inst = inst.Next;
                }
                isFirstInstructionInStructure = false;
            }
        }

        private void WriteStructureFooter(ILBlock s)
        {
            Outdent();
            switch (s.Type)
            {
                case ILBlockType.Loop:
                    WriteToken("}");
                    WriteLine();
                    break;
                case ILBlockType.Try:
                    WriteToken("}");
                    WriteLine();
                    break;
                case ILBlockType.Handler:
                    WriteToken("}");
                    WriteLine();
                    break;
                case ILBlockType.Filter:
                    WriteToken("}");
                    WriteLine();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void WriteStructureHeader(ILBlock s)
        {
            switch (s.Type)
            {
                case ILBlockType.Loop:
                    if (s.LoopEntryPoint != null)
                    {
                        //WriteTokenPreSpace("(");
                        //WriteKeyword("head");
                        //WriteTokenPostSpace(":");
                        //WriteOffsetReference(s.LoopEntryPoint);
                        //WriteToken(")");
                    }
                    WriteKeyword(".loop");
                    WriteOpeningBraceBetweenLines();
                    break;
                case ILBlockType.Try:
                    WriteKeyword(".try");
                    WriteOpeningBraceBetweenLines();
                    break;
                case ILBlockType.Handler:
                    switch (s.ExceptionHandler.HandlerType)
                    {
                        case Mono.Cecil.Cil.ExceptionHandlerType.Catch:
                        case Mono.Cecil.Cil.ExceptionHandlerType.Filter:
                            WriteKeyword("catch");
                            if (s.ExceptionHandler.CatchType != null)
                            {
                                WriteSpace();
                                WriteTypeReference(s.ExceptionHandler.CatchType, ILNameSyntax.TypeName);
                            }
                            WriteLine();
                            break;
                        case Mono.Cecil.Cil.ExceptionHandlerType.Finally:
                            WriteKeyword("finally");
                            WriteLine();
                            break;
                        case Mono.Cecil.Cil.ExceptionHandlerType.Fault:
                            WriteKeyword("fault");
                            WriteLine();
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    WriteToken("{");
                    WriteLine();
                    break;
                case ILBlockType.Filter:
                    WriteKeyword("filter");
                    WriteOpeningBraceBetweenLines();
                    break;
                default:
                    throw new NotSupportedException();
            }
            Indent();
        }

        private void WriteExceptionHandler(ExceptionHandler exceptionHandler)
        {
            WriteKeyWordPostSpace("Try");
            WriteOffsetReference(exceptionHandler.TryStart);
            WriteToken("-");
            WriteOffsetReference(exceptionHandler.TryEnd);
            WriteSpace();
            WriteKeyword(exceptionHandler.HandlerType.ToString());
            if (exceptionHandler.FilterStart != null)
            {
                WriteSpace();
                WriteOffsetReference(exceptionHandler.FilterStart);
                WriteSpace();
                WriteKeyword("handler");
                WriteSpace();
            }
            if (exceptionHandler.CatchType != null)
            {
                WriteSpace();
                WriteTypeReference(exceptionHandler.CatchType);
            }
            WriteSpace();
            WriteOffsetReference(exceptionHandler.HandlerStart);
            WriteSpace();
            WriteOffsetReference(exceptionHandler.HandlerEnd);
        }

        private void WriteInstruction(Instruction instruction)
        {
            WriteDefinition(OffsetToString(instruction.Offset), instruction);
            WriteTokenPostSpace(":");
            WriteReference(instruction.OpCode.Name, instruction.OpCode);
            if (instruction.Operand != null)
            {
                WriteSpace();
                if (instruction.OpCode == OpCodes.Ldtoken)
                {
                    if (instruction.Operand is MethodReference)
                        WriteKeyWordPostSpace("method");
                    else if (instruction.Operand is FieldReference)
                        WriteKeyWordPostSpace("field");
                }
                WriteOperand(instruction.Operand);
            }
        }

        private HashSet<int> GetBranchTargets(IEnumerable<Instruction> instructions)
        {
            HashSet<int> branchTargets = new HashSet<int>();
            foreach (var inst in instructions)
            {
                Instruction target = inst.Operand as Instruction;
                if (target != null)
                    branchTargets.Add(target.Offset);
                Instruction[] targets = inst.Operand as Instruction[];
                if (targets != null)
                    foreach (Instruction t in targets)
                        branchTargets.Add(t.Offset);
            }
            return branchTargets;
        }

        private bool HasParameterAttributes(ParameterDefinition p)
        {
            return p.HasConstant || p.HasCustomAttributes;
        }

        private void WriteParameterAttributes(ParameterDefinition p)
        {
            if (!HasParameterAttributes(p))
                return;
            WriteKeyWordPostSpace(".param");
            WriteToken(string.Format("[{0}]", p.Index + 1));

            if (p.HasConstant)
            {
                WriteSpace();
                WriteToken("=");
                WriteSpace();
                WriteConstant(p.Constant.Value);
            }
            WriteLine();
            WriteAttributes(p.CustomAttributes);
        }

        private void WriteConstant(object constant)
        {
            if (constant == null)
            {
                WriteKeyword("nullref");
            }
            else
            {
                string typeName = ILHelpers.PrimitiveTypeName(constant.GetType().FullName);
                if (typeName != null && typeName != "string")
                {
                    Write(typeName);
                    WriteToken("(");
                    float? cf = constant as float?;
                    double? cd = constant as double?;
                    if (cf.HasValue && (float.IsNaN(cf.Value) || float.IsInfinity(cf.Value)))
                    {
                        Write(string.Format("0x{0:x8}", BitConverter.ToInt32(BitConverter.GetBytes(cf.Value), 0)));
                    }
                    else if (cd.HasValue && (double.IsNaN(cd.Value) || double.IsInfinity(cd.Value)))
                    {
                        Write(string.Format("0x{0:x16}", BitConverter.DoubleToInt64Bits(cd.Value)));
                    }
                    else
                    {
                        WriteOperand(constant);
                    }
                    WriteToken(")");
                }
                else
                {
                    WriteOperand(constant);
                }
            }
        }

        private string OffsetToString(int offset)
        {
            return string.Format("IL_{0:x4}", offset);
        }

        private void WriteOffsetReference(Instruction instruction)
        {
            Write(OffsetToString(instruction.Offset));
        }

        private void WriteLabelList(Instruction[] instructions)
        {
            WriteToken("(");
            for (int i = 0; i < instructions.Length; i++)
            {
                if (i != 0) WriteTokenPostSpace(",");
                WriteOffsetReference(instructions[i]);
            }
            WriteToken(")");
        }

        private void WriteOperand(object operand)
        {
            if (operand == null)
                throw new ArgumentNullException("operand");

            Instruction targetInstruction = operand as Instruction;
            if (targetInstruction != null)
            {
                WriteOffsetReference(targetInstruction);
                return;
            }

            Instruction[] targetInstructions = operand as Instruction[];
            if (targetInstructions != null)
            {
                WriteLabelList(targetInstructions);
                return;
            }

            VariableReference variableRef = operand as VariableReference;
            if (variableRef != null)
            {
                if (string.IsNullOrEmpty(variableRef.Name))
                    WriteReference(variableRef.Index.ToString(), variableRef);
                else
                    WriteReference(ILHelpers.Escape(variableRef.Name), variableRef);
                return;
            }

            ParameterReference paramRef = operand as ParameterReference;
            if (paramRef != null)
            {
                if (method != null && method.Body != null && paramRef == method.Body.ThisParameter)
                {
                    // For some reason MONO leaves the index to the default value of -1.
                    // This happens, when instead of using the 'ldarg.0' instruction is used
                    // the longer 'ldarg 0' form.
                    WriteReference(0.ToString(), paramRef);
                }
                else if (string.IsNullOrEmpty(paramRef.Name))
                    WriteReference(paramRef.Index.ToString(), paramRef);
                else
                    WriteReference(ILHelpers.Escape(paramRef.Name), paramRef);
                return;
            }

            MethodReference methodRef = operand as MethodReference;
            if (methodRef != null)
            {
                WriteMethodReference(methodRef, true);
                return;
            }

            TypeReference typeRef = operand as TypeReference;
            if (typeRef != null)
            {
                WriteTypeReference(typeRef, ILNameSyntax.TypeName);
                return;
            }

            FieldReference fieldRef = operand as FieldReference;
            if (fieldRef != null)
            {
                WriteFieldReference(fieldRef);
                return;
            }

            string s = operand as string;
            if (s != null)
            {
                WriteLiteralInQuotes(ConvertString(s));
            }
            else if (operand is char)
            {
                WriteLiteral(((int)(char)operand).ToString());
            }
            else if (operand is float)
            {
                float val = (float)operand;
                WriteFloatOperand(val);
            }
            else if (operand is double)
            {
                double val = (double)operand;
                WriteDoubleOperand(val);
            }
            else if (operand is bool)
            {
                WriteKeyword((bool)operand ? "true" : "false");
            }
            else
            {
                s = ILHelpers.ToInvariantCultureString(operand);
                Write(s);
            }
        }

        private void WriteDoubleOperand(double val)
        {
            if (val == 0)
            {
                WriteLiteral("0.0");
            }
            else if (double.IsInfinity(val) || double.IsNaN(val))
            {
                byte[] data = BitConverter.GetBytes(val);
                WriteToken("(");
                for (int i = 0; i < data.Length; i++)
                {
                    if (i > 0)
                        WriteSpace();
                    WriteLiteral(data[i].ToString("X2"));
                }
                WriteToken(")");
            }
            else
            {
                WriteLiteral(val.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        private void WriteFloatOperand(float val)
        {
            if (val == 0)
            {
                WriteLiteral("0.0");
            }
            else if (float.IsInfinity(val) || float.IsNaN(val))
            {
                byte[] data = BitConverter.GetBytes(val);
                WriteToken("(");
                for (int i = 0; i < data.Length; i++)
                {
                    if (i > 0)
                        WriteSpace();
                    WriteLiteral(data[i].ToString("X2"));
                }
                WriteToken(")");
            }
            else
            {
                WriteLiteral(val.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        private void WriteFieldReference(FieldReference field)
        {
            WriteTypeReference(field.FieldType, ILNameSyntax.SignatureNoNamedTypeParameters);
            WriteSpace();
            WriteTypeReference(field.DeclaringType, ILNameSyntax.TypeName);
            WriteToken("::");
            WriteReference(ILHelpers.Escape(field.Name), field);
        }

        private bool WriteAttributes(Collection<CustomAttribute> attributes)
        {
            if (attributes.Count == 0)
                return false;

            foreach (CustomAttribute a in attributes)
            {
                WriteKeyWordPostSpace(".custom");
                WriteMethodReference(a.Constructor, true);
                byte[] blob = a.GetBlob();
                if (blob != null)
                {
                    WriteSpace();
                    WriteToken("=");
                    WriteSpace();
                    WriteBlob(blob);
                }
                WriteLine();
            }
            return true;
        }

        private void WriteBlob(byte[] blob)
        {
            WriteToken("(");
            Indent();

            for (int i = 0; i < blob.Length; i++)
            {
                if (i % 16 == 0 && i < blob.Length - 1)
                {
                    WriteLine();
                }
                else
                {
                    WriteSpace();
                }
                Write(blob[i].ToString("x2"));
            }

            WriteLine();
            Outdent();
            WriteToken(")");
        }

        private void WriteParameters(Collection<ParameterDefinition> parameters)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                var p = parameters[i];
                if (p.IsIn)
                    WriteKeyWordPostSpace("[in]");
                if (p.IsOut)
                    WriteKeyWordPostSpace("[out]");
                if (p.IsOptional)
                    WriteKeyWordPostSpace("[opt]");
                WriteTypeReference(p.ParameterType);
                WriteSpace();
                if (p.HasMarshalInfo)
                {
                    WriteMarshalInfo(p.MarshalInfo);
                }
                WriteEscaped(p.Name);
                if (i < parameters.Count - 1)
                    WriteToken(",");
                WriteLine();
            }
        }

        private void WriteTypeParameters(IGenericParameterProvider p)
        {
            if (!p.HasGenericParameters)
                return;

            WriteToken("<");
            for (int i = 0; i < p.GenericParameters.Count; i++)
            {
                if (i > 0)
                    WriteTokenPostSpace(",");

                GenericParameter gp = p.GenericParameters[i];
                WriteGenericTypeParameter(gp);
            }
            WriteToken(">");
        }

        private void WriteGenericTypeParameter(GenericParameter gp)
        {
            if (gp.HasReferenceTypeConstraint)
            {
                WriteKeyWordPostSpace("class");
            }
            else if (gp.HasNotNullableValueTypeConstraint)
            {
                WriteKeyWordPostSpace("valuetype");
            }
            if (gp.HasDefaultConstructorConstraint)
            {
                WriteKeyWordPostSpace(".ctor");
            }
            if (gp.HasConstraints)
            {
                WriteToken("(");
                for (int j = 0; j < gp.Constraints.Count; j++)
                {
                    if (j > 0)
                        WriteTokenPostSpace(",");
                    WriteTypeReference(gp.Constraints[j], ILNameSyntax.TypeName);
                }
                WriteTokenPostSpace(")");
            }
            if (gp.IsContravariant)
            {
                WriteToken("-");
            }
            else if (gp.IsCovariant)
            {
                WriteToken("+");
            }
            WriteEscaped(gp.Name);
        }

        protected void WriteMethodReference(MethodReference method, bool writeAsReference)
        {
            if (method.ExplicitThis)
            {
                WriteKeyWordPostSpace("instance");
                WriteKeyWordPostSpace("explicit");
            }
            else if (method.HasThis)
            {
                WriteKeyWordPostSpace("instance");
            }
            WriteTypeReference(method.FixedReturnType, ILNameSyntax.SignatureNoNamedTypeParameters);
            WriteSpace();
            if (method.DeclaringType != null)
            {
                WriteTypeReference(method.DeclaringType, ILNameSyntax.TypeName);
                WriteToken("::");
            }
            WriteMethodName(method, writeAsReference);
            GenericInstanceMethod gim = method as GenericInstanceMethod;
            if (gim != null)
            {
                WriteToken("<");
                for (int i = 0; i < gim.GenericArguments.Count; i++)
                {
                    if (i > 0)
                        WriteTokenPostSpace(",");
                    WriteTypeReference(gim.GenericArguments[i]);
                }
                WriteToken(">");
            }
            WriteToken("(");
            var parameters = method.Parameters;
            for (int i = 0; i < parameters.Count; ++i)
            {
                if (i > 0) WriteTokenPostSpace(", ");
                WriteTypeReference(parameters[i].ParameterType, ILNameSyntax.SignatureNoNamedTypeParameters);
            }
            WriteToken(")");
        }

        private void WriteMethodName(MethodReference method, bool writeAsReference)
        {
            MethodDefinition md = method as MethodDefinition;
            string methodName;
            if (md != null && md.IsCompilerControlled)
            {
                methodName = ILHelpers.Escape(method.Name + "$PST" + method.MetadataToken.ToInt32().ToString("X8"));
            }
            else
            {
                methodName = ILHelpers.Escape(method.Name);
            }
            if (writeAsReference)
            {
                WriteReference(methodName, method);
            }
            else
            {
                Write(methodName);
            }
        }

        private void WriteTypeReference(TypeReference type, ILNameSyntax syntax = ILNameSyntax.Signature)
        {
            ILNameSyntax syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
            if (type is PinnedType)
            {
                WriteTypeReference(((PinnedType)type).ElementType, syntaxForElementTypes);
                WriteKeyWordPreSpace("pinned");
            }
            else if (type is ArrayType)
            {
                WriteArrayType(type, syntaxForElementTypes);
            }
            else if (type is GenericParameter)
            {
                WriteGenericTypeParameter(type, syntax);
            }
            else if (type is ByReferenceType)
            {
                WriteTypeReference(((ByReferenceType)type).ElementType, syntaxForElementTypes);
                WriteToken("&");
            }
            else if (type is PointerType)
            {
                WriteTypeReference(((PointerType)type).ElementType, syntaxForElementTypes);
                WriteToken("*");
            }
            else if (type is GenericInstanceType)
            {
                WriteTypeReference(type.GetElementType(), syntaxForElementTypes);
                WriteToken("<");
                var arguments = ((GenericInstanceType)type).GenericArguments;
                for (int i = 0; i < arguments.Count; i++)
                {
                    TypeReference toWrite = arguments[i];
                    if (((GenericInstanceType)type).PostionToArgument.ContainsKey(i))
                    {
                        toWrite = ((GenericInstanceType)type).PostionToArgument[i];
                    }
                    if (i > 0)
                        WriteTokenPostSpace(",");
                    WriteTypeReference(toWrite, syntaxForElementTypes);
                }
                WriteToken(">");
            }
            else if (type is OptionalModifierType)
            {
                WriteTypeReference(((OptionalModifierType)type).ElementType, syntax);
                WriteKeyWordPreSpace("modopt");
                WriteToken("(");
                WriteTypeReference(((OptionalModifierType)type).ModifierType, ILNameSyntax.TypeName);
                WriteTokenPostSpace(")");
            }
            else if (type is RequiredModifierType)
            {
                WriteTypeReference(((RequiredModifierType)type).ElementType, syntax);
                WriteKeyWordPreSpace("modreq");
                WriteToken("(");
                WriteTypeReference(((RequiredModifierType)type).ModifierType, ILNameSyntax.TypeName);
                WriteTokenPostSpace(")");
            }
            else
            {
                WriteType(type, syntax);
            }
        }

        protected void WriteType(TypeReference type, ILNameSyntax syntax)
        {
            if (type is ArrayType)
            {
                WriteArrayType(type, syntax);
                return;
            }
            string name = ILHelpers.PrimitiveTypeName(type.GetFriendlyFullName(Language));
            if (syntax == ILNameSyntax.ShortTypeName)
            {
                if (name != null)
                    WriteReference(name, type);
                else
                    WriteReference(ILHelpers.Escape(type.Name), type);
            }
            else if ((syntax == ILNameSyntax.Signature || syntax == ILNameSyntax.SignatureNoNamedTypeParameters) && name != null)
            {
                WriteReference(name, type);
            }
            else
            {
                if (syntax == ILNameSyntax.Signature || syntax == ILNameSyntax.SignatureNoNamedTypeParameters)
                    WriteKeyWordPostSpace(type.IsValueType ? "valuetype" : "class");

                if (type.DeclaringType != null)
                {
                    WriteTypeReference(type.DeclaringType, ILNameSyntax.TypeName);
                    WriteKeyword("/");
                    WriteReference(ILHelpers.Escape(type.Name), type);
                }
                else
                {
                    if (!type.IsDefinition && type.Scope != null && !(type is TypeSpecification))
                    {
                        WriteToken("[");
                        WriteEscaped(type.Scope.Name);
                        WriteToken("]");
                    }
                    WriteReference(ILHelpers.Escape(type.FullName), type);
                }
            }
        }

        private void WriteGenericTypeParameter(TypeReference type, ILNameSyntax syntax)
        {
            WriteToken("!");
            if (((GenericParameter)type).Owner.GenericParameterType == GenericParameterType.Method)
            {
                WriteToken("!");
            }
            if (string.IsNullOrEmpty(type.Name) || type.Name[0] == '!' || syntax == ILNameSyntax.SignatureNoNamedTypeParameters)
            {
                WriteReference(((GenericParameter)type).Position.ToString(), null);
            }
            else
            {
                WriteReference(ILHelpers.Escape(type.Name), null);
            }
        }

        private void WriteArrayType(TypeReference type, ILNameSyntax syntaxForElementTypes)
        {
            ArrayType at = (ArrayType)type;
            WriteTypeReference(at.ElementType, syntaxForElementTypes);
            WriteToken("[");
            for (int i = 0; i < at.Dimensions.Count; i++)
            {
                if (i > 0)
                {
                    WriteTokenPostSpace(",");
                }
                WriteToken(at.Dimensions[i].ToString());
            }
            WriteToken("]");
        }

        private void WriteSecurityDeclarations(ISecurityDeclarationProvider secDeclProvider)
        {
            if (!secDeclProvider.HasSecurityDeclarations)
                return;
            foreach (var secdecl in secDeclProvider.SecurityDeclarations)
            {
                WriteKeyWordPostSpace(".permissionset");
                switch (secdecl.Action)
                {
                    case SecurityAction.Request:
                        WriteKeyword("request");
                        break;
                    case SecurityAction.Demand:
                        WriteKeyword("demand");
                        break;
                    case SecurityAction.Assert:
                        WriteKeyword("assert");
                        break;
                    case SecurityAction.Deny:
                        WriteKeyword("deny");
                        break;
                    case SecurityAction.PermitOnly:
                        WriteKeyword("permitonly");
                        break;
                    case SecurityAction.LinkDemand:
                        WriteKeyword("linkcheck");
                        break;
                    case SecurityAction.InheritDemand:
                        WriteKeyword("inheritcheck");
                        break;
                    case SecurityAction.RequestMinimum:
                        WriteKeyword("reqmin");
                        break;
                    case SecurityAction.RequestOptional:
                        WriteKeyword("reqopt");
                        break;
                    case SecurityAction.RequestRefuse:
                        WriteKeyword("reqrefuse");
                        break;
                    case SecurityAction.PreJitGrant:
                        WriteKeyword("prejitgrant");
                        break;
                    case SecurityAction.PreJitDeny:
                        WriteKeyword("prejitdeny");
                        break;
                    case SecurityAction.NonCasDemand:
                        WriteKeyword("noncasdemand");
                        break;
                    case SecurityAction.NonCasLinkDemand:
                        WriteKeyword("noncaslinkdemand");
                        break;
                    case SecurityAction.NonCasInheritance:
                        WriteKeyword("noncasinheritance");
                        break;
                    default:
                        WriteKeyword(secdecl.Action.ToString());
                        break;
                }
                WriteTokenPreSpace("=");
                WriteTokenPreSpace("{");
                WriteLine();
                Indent();
                for (int i = 0; i < secdecl.SecurityAttributes.Count; i++)
                {
                    SecurityAttribute sa = secdecl.SecurityAttributes[i];
                    if (sa.AttributeType.Scope == sa.AttributeType.Module)
                    {
                        WriteKeyWordPostSpace("class");
                        WriteEscaped(GetAssemblyQualifiedName(sa.AttributeType));
                    }
                    else
                    {
                        WriteTypeReference(sa.AttributeType, ILNameSyntax.TypeName);
                    }
                    WriteTokenPreSpace("=");
                    WriteTokenPreSpace("{");
                    if (sa.HasFields || sa.HasProperties)
                    {
                        WriteLine();
                        Indent();

                        foreach (CustomAttributeNamedArgument na in sa.Fields)
                        {
                            WriteKeyWordPostSpace("field");
                            WriteSecurityDeclarationArgument(na);
                            WriteLine();
                        }

                        foreach (CustomAttributeNamedArgument na in sa.Properties)
                        {
                            WriteKeyWordPostSpace("property");
                            WriteSecurityDeclarationArgument(na);
                            WriteLine();
                        }

                        Outdent();
                    }
                    WriteToken("}");

                    if (i + 1 < secdecl.SecurityAttributes.Count)
                        WriteToken(",");
                    WriteLine();
                }
                Outdent();
                WriteToken("}");
                WriteLine();
            }
        }

        private void WriteSecurityDeclarationArgument(CustomAttributeNamedArgument na)
        {
            TypeReference type = na.Argument.Type;
            if (type.MetadataType == MetadataType.Class || type.MetadataType == MetadataType.ValueType)
            {
                WriteKeyWordPostSpace("enum");
                if (type.Scope != type.Module)
                {
                    WriteKeyWordPostSpace("class");
                    Write(ILHelpers.Escape(GetAssemblyQualifiedName(type)));
                }
                else
                {
                    WriteTypeReference(type, ILNameSyntax.TypeName);
                }
            }
            else
            {
                WriteTypeReference(type);
            }
            WriteSpace();
            WriteReference(ILHelpers.Escape(na.Name), type);
            WriteSpace();
            WriteToken("=");
            WriteSpace();
            if (na.Argument.Value is string)
            {
                Write(string.Format("string('{0}')", ConvertString((string)na.Argument.Value).Replace("'", "\'")));
            }
            else
            {
                WriteConstant(na.Argument.Value);
            }
        }

        private string GetAssemblyQualifiedName(TypeReference type)
        {
            AssemblyNameReference anr = type.Scope as AssemblyNameReference;
            if (anr == null)
            {
                ModuleDefinition md = type.Scope as ModuleDefinition;
                if (md != null)
                {
                    anr = md.Assembly.Name;
                }
            }
            if (anr != null)
            {
                return type.FullName + ", " + anr.FullName;
            }
            else
            {
                return type.FullName;
            }
        }

        private void WriteMarshalInfo(MarshalInfo marshalInfo)
        {
            WriteKeyword("marshal");
            WriteToken("(");
            WriteNativeType(marshalInfo.NativeType, marshalInfo);
            WriteToken("(");
            WriteSpace();
        }

        private void WriteNativeType(NativeType nativeType, MarshalInfo marshalInfo = null)
        {
            switch (nativeType)
            {
                case NativeType.None:
                    break;
                case NativeType.Boolean:
                    WriteKeyword("bool");
                    break;
                case NativeType.I1:
                    WriteKeyword("int8");
                    break;
                case NativeType.U1:
                    WriteKeyword("unsigned int8");
                    break;
                case NativeType.I2:
                    WriteKeyword("int16");
                    break;
                case NativeType.U2:
                    WriteKeyword("unsigned int16");
                    break;
                case NativeType.I4:
                    WriteKeyword("int32");
                    break;
                case NativeType.U4:
                    WriteKeyword("unsigned int32");
                    break;
                case NativeType.I8:
                    WriteKeyword("int64");
                    break;
                case NativeType.U8:
                    WriteKeyword("unsigned int64");
                    break;
                case NativeType.R4:
                    WriteKeyword("float32");
                    break;
                case NativeType.R8:
                    WriteKeyword("float64");
                    break;
                case NativeType.LPStr:
                    WriteKeyword("lpstr");
                    break;
                case NativeType.Int:
                    WriteKeyword("int");
                    break;
                case NativeType.UInt:
                    WriteKeyword("unsigned int");
                    break;
                case NativeType.Func:
                    goto default;
                case NativeType.Array:
                    ArrayMarshalInfo ami = (ArrayMarshalInfo)marshalInfo;
                    if (ami == null)
                        goto default;
                    if (ami.ElementType != NativeType.Max)
                        WriteNativeType(ami.ElementType);
                    WriteToken("[");
                    if (ami.SizeParameterMultiplier == 0)
                    {
                        WriteLiteral(ami.Size.ToString());
                    }
                    else
                    {
                        if (ami.Size >= 0)
                            WriteLiteral(ami.Size.ToString());
                        WriteSpace();
                        WriteToken("+");
                        WriteSpace();
                        WriteLiteral(ami.SizeParameterIndex.ToString());
                    }
                    WriteToken("]");
                    break;
                case NativeType.Currency:
                    WriteKeyword("currency");
                    break;
                case NativeType.BStr:
                    WriteKeyword("bstr");
                    break;
                case NativeType.LPWStr:
                    WriteKeyword("lpwstr");
                    break;
                case NativeType.LPTStr:
                    WriteKeyword("lptstr");
                    break;
                case NativeType.FixedSysString:
                    WriteKeyword(string.Format("fixed sysstring[{0}]", ((FixedSysStringMarshalInfo)marshalInfo).Size));
                    break;
                case NativeType.IUnknown:
                    WriteKeyword("iunknown");
                    break;
                case NativeType.IDispatch:
                    WriteKeyword("idispatch");
                    break;
                case NativeType.Struct:
                    WriteKeyword("struct");
                    break;
                case NativeType.IntF:
                    WriteKeyword("interface");
                    break;
                case NativeType.SafeArray:
                    WriteKeyWordPostSpace("safearray");
                    SafeArrayMarshalInfo sami = marshalInfo as SafeArrayMarshalInfo;
                    if (sami != null)
                    {
                        switch (sami.ElementType)
                        {
                            case VariantType.None:
                                break;
                            case VariantType.I2:
                                WriteKeyword("int16");
                                break;
                            case VariantType.I4:
                                WriteKeyword("int32");
                                break;
                            case VariantType.R4:
                                WriteKeyword("float32");
                                break;
                            case VariantType.R8:
                                WriteKeyword("float64");
                                break;
                            case VariantType.CY:
                                WriteKeyword("currency");
                                break;
                            case VariantType.Date:
                                WriteKeyword("date");
                                break;
                            case VariantType.BStr:
                                WriteKeyword("bstr");
                                break;
                            case VariantType.Dispatch:
                                WriteKeyword("idispatch");
                                break;
                            case VariantType.Error:
                                WriteKeyword("error");
                                break;
                            case VariantType.Bool:
                                WriteKeyword("bool");
                                break;
                            case VariantType.Variant:
                                WriteKeyword("variant");
                                break;
                            case VariantType.Unknown:
                                WriteKeyword("iunknown");
                                break;
                            case VariantType.Decimal:
                                WriteKeyword("decimal");
                                break;
                            case VariantType.I1:
                                WriteKeyword("int8");
                                break;
                            case VariantType.UI1:
                                WriteKeyword("unsigned int8");
                                break;
                            case VariantType.UI2:
                                WriteKeyword("unsigned int16");
                                break;
                            case VariantType.UI4:
                                WriteKeyword("unsigned int32");
                                break;
                            case VariantType.Int:
                                WriteKeyword("int");
                                break;
                            case VariantType.UInt:
                                WriteKeyword("unsigned int");
                                break;
                            default:
                                WriteKeyword(sami.ElementType.ToString());
                                break;
                        }
                    }
                    break;
                case NativeType.FixedArray:
                    WriteKeyword("fixed array");
                    FixedArrayMarshalInfo fami = marshalInfo as FixedArrayMarshalInfo;
                    if (fami != null)
                    {
                        WriteToken("[");
                        WriteLiteral(fami.Size.ToString());
                        WriteToken("]");
                        if (fami.ElementType != NativeType.None)
                        {
                            WriteSpace();
                            WriteNativeType(fami.ElementType);
                        }
                    }
                    break;
                case NativeType.ByValStr:
                    WriteKeyword("byvalstr");
                    break;
                case NativeType.ANSIBStr:
                    WriteKeyword("ansi bstr");
                    break;
                case NativeType.TBStr:
                    WriteKeyword("tbstr");
                    break;
                case NativeType.VariantBool:
                    WriteKeyword("variant bool");
                    break;
                case NativeType.ASAny:
                    WriteKeyword("as any");
                    break;
                case NativeType.LPStruct:
                    WriteKeyword("lpstruct");
                    break;
                case NativeType.CustomMarshaler:
                    CustomMarshalInfo cmi = marshalInfo as CustomMarshalInfo;
                    if (cmi == null)
                        goto default;
                    WriteKeyword("custom");
                    WriteToken("(");
                    WriteLiteralInQuotes(ConvertString(cmi.ManagedType.FullName));
                    WriteTokenPostSpace(",");
                    WriteLiteralInQuotes(ConvertString(cmi.Cookie));
                    if (cmi.Guid != Guid.Empty || !string.IsNullOrEmpty(cmi.UnmanagedType))
                    {
                        WriteTokenPostSpace(",");
                        WriteLiteralInQuotes(cmi.Guid.ToString());
                        WriteTokenPostSpace(",");
                        WriteLiteralInQuotes(ConvertString(cmi.UnmanagedType));
                    }
                    WriteToken(")");
                    break;
                case NativeType.Error:
                    WriteKeyword("error");
                    break;
                default:
                    WriteKeyword(nativeType.ToString());
                    break;
            }
        }

        public IntermediateLanguageWriter(ILanguage language, IFormatter formatter, IExceptionFormatter exceptionFormatter, IWriterSettings settings)
            : base(language, formatter, exceptionFormatter, settings)
        {
            this.flagsWriter = new FlagsWriter(this);
        }

        protected enum ILNameSyntax
        {
            Signature,
            SignatureNoNamedTypeParameters,
            TypeName,
            ShortTypeName
        }
    }

    class CodeMappings
    {
        public string FullName { get; set; }

        public List<MemberMapping> Mapping { get; set; }
    }

    class ILRange
    {
        public int From;
        public int To;

        public override string ToString()
        {
            return string.Format("{0}-{1}", From.ToString("X"), To.ToString("X"));
        }

        public static List<ILRange> OrderAndJoint(IEnumerable<ILRange> input)
        {
            if (input == null)
                throw new ArgumentNullException("Input is null!");

            List<ILRange> ranges = input.Where(r => r != null).OrderBy(r => r.From).ToList();
            for (int i = 0; i < ranges.Count - 1; )
            {
                ILRange curr = ranges[i];
                ILRange next = ranges[i + 1];

                if (curr.From <= next.From && next.From <= curr.To)
                {
                    curr.To = Math.Max(curr.To, next.To);
                    ranges.RemoveAt(i + 1);
                }
                else
                {
                    i++;
                }
            }
            return ranges;
        }

        public static IEnumerable<ILRange> Invert(IEnumerable<ILRange> input, int codeSize)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (codeSize <= 0)
                throw new ArgumentException("Code size must be grater than 0");

            var ordered = OrderAndJoint(input);
            if (ordered.Count == 0)
            {
                yield return new ILRange() { From = 0, To = codeSize };
            }
            else
            {
                if (ordered.First().From != 0)
                    yield return new ILRange() { From = 0, To = ordered.First().From };

                for (int i = 0; i < ordered.Count - 1; i++)
                    yield return new ILRange() { From = ordered[i].To, To = ordered[i + 1].From };

                Debug.Assert(ordered.Last().To <= codeSize);
                if (ordered.Last().To != codeSize)
                    yield return new ILRange() { From = ordered.Last().To, To = codeSize };
            }
        }
    }

    internal sealed class SourceCodeMapping
    {
        internal ILRange ILInstructionOffset { get; set; }

        internal MemberMapping MemberMapping { get; set; }
    }

    internal sealed class MemberMapping
    {
        IEnumerable<ILRange> invertedList;

        public MemberReference MemberReference { get; internal set; }

        public uint MetadataToken { get; internal set; }

        public int CodeSize { get; internal set; }

        internal List<SourceCodeMapping> MemberCodeMappings { get; set; }

        public IEnumerable<ILRange> InvertedList
        {
            get
            {
                if (invertedList == null)
                {
                    var list = MemberCodeMappings.ConvertAll<ILRange>(
                        s => new ILRange { From = s.ILInstructionOffset.From, To = s.ILInstructionOffset.To });
                    invertedList = ILRange.OrderAndJoint(ILRange.Invert(list, CodeSize));
                }
                return invertedList;
            }
        }
    }
}