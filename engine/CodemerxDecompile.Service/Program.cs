//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

using CodemerxDecompile.Service.Extensions;
using CodemerxDecompile.Service.Interfaces;

namespace CodemerxDecompile.Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            CommandLineParameters parameters = CommandLineParameters.Parse(args);

            IHost host = CreateHostBuilder(args, parameters).Build();

            IPathService pathService = host.Services.GetService<IPathService>();
            pathService.WorkingDirectory = parameters.WorkingDir;

            IServiceManager serviceManager = host.Services.GetService<IServiceManager>();

            await host.RunAsync(serviceManager.CancellationToken);
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args, CommandLineParameters parameters)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        if (parameters.HasPort)
                        {
                            // Setup a HTTP/2 endpoint without TLS.
                            options.ListenLocalhost(int.Parse(parameters.Port), o => o.Protocols = HttpProtocols.Http2);
                        }
                    });

                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
