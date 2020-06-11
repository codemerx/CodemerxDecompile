using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.IO;

namespace JustDecompile.External.JustAssembly
{
    class CodeViewerResults : ICodeViewerResults
    {
		public CodeViewerResults(JustDecompile.EngineInfrastructure.DecompiledSourceCode instance)
		{
			Tuple<int, ITokenProvider>[] lineToMemberTokenMapList = new Tuple<int, ITokenProvider>[instance.LineToMemberMapList.Count];
			for (int i = 0; i < lineToMemberTokenMapList.Length; i++)
			{
				Tuple<int, IMemberDefinition> lineToMember = instance.LineToMemberMapList[i];
				IMemberDefinition member = lineToMember.Item2;

				if (member is TypeDefinition)
				{
					TypeDefinition type = member as TypeDefinition;
					lineToMemberTokenMapList[i] = new Tuple<int, ITokenProvider>(lineToMember.Item1, new TypeTokenProvider(type.MetadataToken.ToUInt32()));
				}
				else
				{
					lineToMemberTokenMapList[i] =
						new Tuple<int, ITokenProvider>(lineToMember.Item1, new MemberTokenProvider(member.DeclaringType.MetadataToken.ToUInt32(), member.MetadataToken.ToUInt32()));
				}
			}
			this.LineToMemberTokenMap = lineToMemberTokenMapList;

			this.NewLine = instance.NewLine;
		}

		public IEnumerable<Tuple<int, ITokenProvider>> LineToMemberTokenMap { get; private set; }

		public string NewLine { get; private set; }
	}
}
