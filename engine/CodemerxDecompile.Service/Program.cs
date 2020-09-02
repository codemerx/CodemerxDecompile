using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CodemerxDecompile.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CommandLineParameters parameters = CommandLineParameters.Parse(args);

            CreateHostBuilder(args, parameters).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args, CommandLineParameters parameters)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    if (parameters.HasPort)
                    {
                        webBuilder.UseUrls($"http://localhost:{parameters.Port}/");
                    }
                });
        }
    }
}
