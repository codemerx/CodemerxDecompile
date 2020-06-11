using Telerik.JustDecompiler.Languages.CSharp;

namespace Telerik.JustDecompiler.Languages
{
    public static partial class LanguageFactory
    {
        private class CSharpV7 : CSharpV6, ICSharp
        {
            private static CSharpV7 instance;

            static CSharpV7()
            {
                instance = new CSharpV7();
            }

            protected CSharpV7()
            {
            }

            new public static CSharpV7 Instance
            {
                get
                {
                    return instance;
                }
            }

            public override int Version
            {
                get
                {
                    return 7;
                }
            }
        }
    }
}
