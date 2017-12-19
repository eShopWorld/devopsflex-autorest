using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;

namespace ESW.autorest.createProject
{
    public class CommandLineOptions
    {
        /// <summary>
        /// indicates whether help is to be displayed
        /// </summary>
        public bool IsHelp { get; set; }
        
        /// <summary>
        /// flag to contain options validation
        /// </summary>
        public bool ValidationSucceeded { get; set; } = true;

        /// <summary>
        /// list of validation errors
        /// </summary>
        public IList<string> ValidationErrors { get; set; }


        /// <summary>
        /// url to the swagger json file
        /// </summary>
        public string SwaggerJsonUrl { get; set; }

        /// <summary>
        /// output folder path to generate files into
        /// </summary>
        public string OutputFolder { get; set; }
        /// <summary>
        /// target framework moniker
        /// 
        /// with netstandard 1.5 we cover both full framework as well as .net core apps
        /// </summary>
        public List<string> TFMs { get; set; }

        public static CommandLineOptions Parse(string[] args)
        {
            var app = new CommandLineApplication(false)
            {
                Name = "dotnet autorest-createproject",
                FullName = "eShopWorld autorest wrapper (C#)"
            };           

            var options = new CommandLineOptions();

            options.Configure(app);

            app.Execute(args);
            options.IsHelp = app.IsShowingInformation;            

            return options;
        }

        /// <summary>
        /// configure command prompt options applicable to this util
        /// </summary>
        /// <param name="app">application wrapper instance</param>
        private void Configure(CommandLineApplication app)
        {
            //help
            var help = app.HelpOption("-h|--help");

            //target folder
            var swaggerFileOption = app.Option("-s|--swagger <name>",
                "url to the swagger JSON file",
                CommandOptionType.SingleValue);

            var outputFolderOption = app.Option("-o|--output <outputFolder>",
                "output folder path to generate files into",
                CommandOptionType.SingleValue);

            var tfmOption = app.Option("-t|--tfm <tfm>", "Target framework moniker name (default: netstandard1.5)",
                CommandOptionType.MultipleValue);
       
            app.OnExecute(() =>
            {
                //validation first for required params
                if (!swaggerFileOption.HasValue())
                {
                    ValidationSucceeded = false;
                    ValidationErrors = new List<string>(new[]
                        {"Required parameter 'swagger file' missing (-s). See help (-h) for details"});
                    return -1;
                }

                if (!outputFolderOption.HasValue())
                {
                    ValidationSucceeded = false;
                    ValidationErrors = new List<string>(new[]
                        {"Required parameter 'output' missing (-o). See help (-h) for details"});
                    return -1;
                }

                //pass back to execution
                IsHelp = help.HasValue();
                SwaggerJsonUrl = swaggerFileOption.Value();
                OutputFolder = outputFolderOption.Value();
                TFMs = tfmOption.Values==null || tfmOption.Values.Count==0 ? new [] { "net462","netstandard2.0" }.ToList() : tfmOption.Values;
                return 0;
            });
        }
    }
}
