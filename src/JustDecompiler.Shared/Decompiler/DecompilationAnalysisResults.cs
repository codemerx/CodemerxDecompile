using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler
{
	public class DecompilationAnalysisResults
	{
		/// <summary>
		/// Contains a list of types the current method depends on.
		/// </summary>
		public HashSet<TypeReference> TypesDependingOn { get; private set; }

        /// <summary>
        /// Contains a list of cast expressions from the current method, which have unresolved references up the inheritance chain that we need to find out whether == or != is overloaded.
        /// </summary>
        public HashSet<ExplicitCastExpression> AmbiguousCastsToObject { get; private set; }

		public DecompilationAnalysisResults()
		{
			this.TypesDependingOn = new HashSet<TypeReference>();
            this.AmbiguousCastsToObject = new HashSet<ExplicitCastExpression>();
		}
	}
}
