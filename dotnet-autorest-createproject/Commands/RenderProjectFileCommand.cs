using System;
using ESW.autorest.createProject.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ESW.autorest.createProject.Commands
{
    public class RenderProjectFileCommand : RazorRenderCommandBase<ProjectFileViewModel>
    {
        public RenderProjectFileCommand(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider):base(viewEngine, tempDataProvider, serviceProvider)
        {            
        }

        public override void Render(ProjectFileViewModel model, string outputFile)
        {
            RenderViewToString(outputFile, model, "Views\\ProjectFile.cshtml");
        }
    }
}
