namespace CodemerxDecompile;

public static class EnvironmentProvider
{
    public static Environment Environment =>
#if DEBUG
        Environment.Development;
#else
        Environment.Production;
#endif
}
