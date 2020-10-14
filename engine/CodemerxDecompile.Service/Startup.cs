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

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using CodemerxDecompile.Service.Interfaces;
using CodemerxDecompile.Service.Services;
using CodemerxDecompile.Service.Services.DecompilationContext;
using CodemerxDecompile.Service.Services.Search;

namespace CodemerxDecompile.Service
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddSingleton<ISearchService, SearchService>();
            services.AddSingleton<IDecompilationContextService, DecompilationContextService>();
            services.AddSingleton<IServiceManager, ServiceManager>();
            services.AddSingleton<IPathService, PathService>();
            services.AddSingleton<IStorageService, FileStorageService>();
            services.AddSingleton<IDecompilationContext, JsonSerializableDecompilationContext>();
            services.AddSingleton<ISerializationService, JsonSerializationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<RpcDecompilerService>();
                endpoints.MapGrpcService<RpcManagerService>();
            });

            IServiceProvider serviceProvider = app.ApplicationServices;

            applicationLifetime.ApplicationStarted.Register(() => this.InitializeServices(serviceProvider));
            applicationLifetime.ApplicationStopping.Register(() => this.DisposeServices(serviceProvider));
        }

        private void InitializeServices(IServiceProvider services)
        {
            IStorageService storageService = services.GetService<IStorageService>();
            if (storageService.HasStored<IDecompilationContext>())
            {
                IDecompilationContext decompilationContext = storageService.Retrieve<IDecompilationContext, JsonSerializableDecompilationContext>();

                IDecompilationContextService decompilationContextService = services.GetService<IDecompilationContextService>();
                decompilationContextService.DecompilationContext = decompilationContext;
            }
        }

        private void DisposeServices(IServiceProvider services)
        {
            IStorageService storageService = services.GetService<IStorageService>();
            IDecompilationContextService decompilationContextService = services.GetService<IDecompilationContextService>();

            storageService.Store(decompilationContextService.DecompilationContext);
        }
    }
}
