using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ESW.autorest.createProject.Commands;
using ESW.autorest.createProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.PlatformAbstractions;

namespace ESW.autorest.createProject
{
    public class Loader
    {
        public static void Main(string[] args)
        {      
            try
            {

                var options = CommandLineOptions.Parse(args);

                if (options.IsHelp)
                    return;

                if (!options.ValidationSucceeded)
                {
                    foreach (var error in options?.ValidationErrors)
                        Console.Out.WriteLine(error);

                    return;
                }

                // Initialize the necessary services
                var services = new ServiceCollection();
                ConfigureDefaultServices(services, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                var provider = services.BuildServiceProvider();
                var serviceScope = provider.GetRequiredService<IServiceScopeFactory>();
                using (serviceScope.CreateScope())
                {
                    var jsonFilePath = options.SwaggerJsonPath;
                    var swaggerInfo = SwaggerJsonParser.ParsetOut(jsonFilePath);
                    var projectFileName = swaggerInfo.Item1 + ".csproj";
                    
                    //generate project file
                    var projectFileCommand = provider.GetRequiredService<RenderProjectFileCommand>();                    
                    projectFileCommand.Render(new ProjectFileViewModel{TFM = options.TFM, ProjectName = swaggerInfo.Item1, Version = swaggerInfo.Item2}, Path.Combine(options.OutputFolder, projectFileName));
                    //generate SLN file
                    var slnFileCommand = provider.GetRequiredService<RenderSLNFileCommand>();
                    slnFileCommand.Render(new SLNFileViewModel{ProjectFileName = projectFileName, ProjectName = swaggerInfo.Item1}, Path.Combine(options.OutputFolder, swaggerInfo.Item1+ ".sln"));
                }           
            }
            catch (Exception)
            {
#if DEBUG
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
#endif
                throw;
            }
        }

        private static void ConfigureDefaultServices(IServiceCollection services, string customApplicationBasePath)
        {
            var applicationEnvironment = PlatformServices.Default.Application;

            services.AddSingleton(applicationEnvironment);
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();        

            IFileProvider fileProvider;
            string applicationName;
            if (!string.IsNullOrEmpty(customApplicationBasePath))
            {
                applicationName = Assembly.GetEntryAssembly().GetName().Name;
                fileProvider = new PhysicalFileProvider(customApplicationBasePath);
            }
            else
            {
                applicationName = Assembly.GetEntryAssembly().GetName().Name;
                fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            }

            services.AddSingleton<IHostingEnvironment>(new HostingEnvironment
            {
                ApplicationName = applicationName,
                WebRootFileProvider = fileProvider,
            });

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(fileProvider);
            });

            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddLogging();
            services.AddMvc();
            services.AddTransient<RenderProjectFileCommand>();
            services.AddTransient<RenderSLNFileCommand>();
        }
    }
}
