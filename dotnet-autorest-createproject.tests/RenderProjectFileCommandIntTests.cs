using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using ESW.autorest.createProject.Commands;
using ESW.autorest.createProject.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ESW.autorest.createProject.tests
{    
    public class RenderProjectFileCommandIntTests
    {        
        private readonly IServiceProvider serviceProvider;

        public RenderProjectFileCommandIntTests()
        {
            var services = new ServiceCollection();
            Runner.ConfigureDefaultServices(services, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));       
            serviceProvider = services.BuildServiceProvider();                      
        }

        [Fact, Trait("Category", "Integration")]
        public void RenderView_GeneratesExpectedContent()
        {
            
            var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (serviceScope.CreateScope())
            {
                var sut = serviceProvider.GetRequiredService<RenderProjectFileCommand>();

                //generate project file                
                var output = sut.RenderViewToString(new ProjectFileViewModel{ProjectName = "test project", TFM = "testTFM", Version = "1.2.3.4"}, "Views\\TestTemplate.cshtml");
                output.Should().Be("lorem ipsum tfm=testTFM version=1.2.3.4 projectName=test project");
            }
        }
    }
}
