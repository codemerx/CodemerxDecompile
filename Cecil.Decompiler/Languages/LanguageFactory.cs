using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Languages.CSharp;
using Telerik.JustDecompiler.Languages.VisualBasic;

namespace Telerik.JustDecompiler.Languages
{
    public static partial class LanguageFactory
    {
        public static ILanguage GetLanguage(CSharpVersion version)
        {
            switch (version)
            {
                case CSharpVersion.None:
                    return CSharp.Instance;
                case CSharpVersion.V5:
                    return CSharpV5.Instance;
                case CSharpVersion.V6:
                    return CSharpV6.Instance;
                case CSharpVersion.V7:
                    return CSharpV7.Instance;
                default:
                    throw new ArgumentException();
            }
        }

        public static ILanguage GetLanguage(VisualBasicVersion version)
        {
            switch (version)
            {
                case VisualBasicVersion.None:
                    return VisualBasic.Instance;
                case VisualBasicVersion.V10:
                    return VisualBasicV10.Instance;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
